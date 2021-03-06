using System;
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
            if (!terrainGeneratorData.noiseData.Validate())
                return;

            Start(terrainGridData.terrains.Count);

            int resolution = terrainGridData.terrains[0].terrainData.heightmapResolution;

            int i = 0;
            int count = terrainGridData.gridSideCount;

            float size = ((float)resolution - 1) * count;

            int tileOffsetStep = resolution - 1;
            float tileOffsetHalf = (float)tileOffsetStep / 2;
            float tileOffsetFull = (count - 1) * tileOffsetHalf;

            float2 tileOffset;

            if (count == 1)
                tileOffset = new float2(0, 0);
            else if (count == 2)
                tileOffset = new float2(tileOffsetHalf, -tileOffsetHalf);
            else
                tileOffset = new float2(tileOffsetFull, -tileOffsetFull);

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
                            terrainGridData.terrains[(int)args[1]].terrainData.SetHeights(0, 0, generatedMap);
                            CompleteTask();
                        }, 
                        null);
                    }, 
                    new object[] { tileOffset, i });

                    tileOffset.y += tileOffsetStep;
                    i++;
                }
                tileOffset.y = -tileOffsetFull;
                tileOffset.x -= tileOffsetStep;
            }
        }

        private static Func<float2, float[,]> GetHeightMapFunc(int resolution, float size, NoiseData noiseData, FalloffData falloffData)
        {
            if (falloffData.useFalloff)
            {
                return (tileOffset) =>
                {
                    float[,] noiseMap = NoiseMap.Generate(resolution, noiseData, tileOffset);
                    float[,] falloffMap = FalloffMap.Generate(resolution, size, tileOffset, falloffData);

                    for (int y = 0; y < resolution; y++)
                        for (int x = 0; x < resolution; x++)
                            noiseMap[x, y] *= falloffMap[x, y];

                    /*
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
                    */

                    return noiseMap;
                };
            }
            else
                return (tileOffset) => NoiseMap.Generate(resolution, noiseData, tileOffset);
        }

        public static void GenerateSplatMap(TerrainGridData terrainGridData, TerrainGeneratorData terrainGeneratorData)
        {
            if (!terrainGeneratorData.splatMapData.Validate(terrainGridData.terrains[0].terrainData.alphamapLayers))
                return;

            Start(terrainGridData.terrains.Count);

            int count = (int)math.pow(terrainGridData.gridSideCount, 2);

            for (int i = 0; i < count; i++)
            {
                float[,,] generatedMap = SplatMap.Generate(terrainGridData.terrains[i].terrainData, terrainGeneratorData.splatMapData);

                terrainGridData.terrains[i].terrainData.SetAlphamaps(0, 0, generatedMap);
                CompleteTask();
            }
        }
    }
}
