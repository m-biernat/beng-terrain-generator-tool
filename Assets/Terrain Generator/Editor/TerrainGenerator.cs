﻿using System;
using System.Threading;
using Unity.Mathematics;

namespace TerrainGenerator 
{
    public static class TerrainGenerator
    {
        #region ProgressTracking
        public static bool isWorking = false;
        public static int completedTasks, scheduledTasks;

        public static Action onStart, onComplete;

        public static void Start(int numOfTasks)
        {
            isWorking = true;
            completedTasks = 0;
            scheduledTasks = numOfTasks;

            onStart?.Invoke();
        }

        public static void CompleteTask()
        {
            completedTasks++;

            if (completedTasks == scheduledTasks)
            {
                isWorking = false;
                onComplete?.Invoke();
            }
        }
        #endregion

        public static void GenerateHeightMap(TerrainGridData terrainGridData, TerrainGeneratorData terrainGeneratorData)
        {
            Start(terrainGridData.terrain.Count);

            int resolution = terrainGridData.terrain[0].terrainData.heightmapResolution;

            int i = 0;
            int count = terrainGridData.gridSideCount;

            float size = ((float)resolution - 1) * count;

            int falloffDiff = resolution - 1;
            float falloffBase = (float)falloffDiff / 2;
            float tileOffsetValue = (count - 1) * falloffBase;

            float2 tileOffset;

            if (count == 1)
                tileOffset = new float2(0, 0);
            else if (count == 2)
                tileOffset = new float2(falloffBase, -falloffBase);
            else
                tileOffset = new float2(tileOffsetValue, -tileOffsetValue);

            Func<float2, float[,]> getHeightMap = 
                GetHeightMapFunc(resolution, size, terrainGeneratorData.noiseData, terrainGeneratorData.falloffData);

            var syncContext = SynchronizationContext.Current;

            for (int y = 0; y < count; y++)
            {
                for (int x = 0; x < count; x++)
                {
                    ThreadPool.QueueUserWorkItem((input) => 
                    {
                        object[] args = input as object[];

                        float[,] generatedMap = getHeightMap((float2)args[0]);

                        syncContext.Post(_ =>
                        {
                            terrainGridData.terrain[(int)args[1]].terrainData.SetHeights(0, 0, generatedMap);
                            CompleteTask();
                        }, 
                        null);
                    }, 
                    new object[] { tileOffset, i });

                    /*
                    generatedMap = getHeightMap(tileOffset);

                    terrainGridData.terrain[i].terrainData.SetHeights(0, 0, generatedMap);
                    */

                    tileOffset.y += falloffDiff;
                    i++;
                }
                tileOffset.y = -tileOffsetValue;
                tileOffset.x -= falloffDiff;
            }
        }

        private static Func<float2, float[,]> GetHeightMapFunc(int resolution, float size, NoiseData noiseData, FalloffData falloffData)
        {
            if (falloffData.useFalloff)
            {
                return (tileOffset) =>
                {
                    float[,] noiseMap = NoiseMap.Generate(resolution, noiseData, tileOffset);

                    Func<int, int, float> getFalloff;

                    if (falloffData.type == FalloffType.Rectangular)
                    {
                        Falloff.RectangularData rectangularData =
                        new Falloff.RectangularData(resolution, size, tileOffset);

                        getFalloff = (x, y) => Falloff.GetRectangular(x, y, size, falloffData, rectangularData);
                    }
                    else
                    {
                        Falloff.RadialData radialData =
                        new Falloff.RadialData(resolution, size, tileOffset);

                        getFalloff = (x, y) => Falloff.GetRadial(x, y, size, falloffData, radialData);
                    }

                    for (int y = 0; y < resolution; y++)
                        for (int x = 0; x < resolution; x++)
                            noiseMap[x, y] *= getFalloff(x, y);

                    return noiseMap;
                };
            }
            else
                return (tileOffset) => NoiseMap.Generate(resolution, noiseData, tileOffset);
        }

        public static void GenerateSplatMap(TerrainGridData terrainGridData, TerrainGeneratorData terrainGeneratorData)
        {
            Start(terrainGridData.terrain.Count);

            int count = (int)math.pow(terrainGridData.gridSideCount, 2);

            for (int i = 0; i < count; i++)
            {
                float[,,] generatedMap = SplatMap.Generate(terrainGridData.terrain[i].terrainData, terrainGeneratorData.splatMapData);

                terrainGridData.terrain[i].terrainData.SetAlphamaps(0, 0, generatedMap);
                CompleteTask();
            }
        }
    }
}