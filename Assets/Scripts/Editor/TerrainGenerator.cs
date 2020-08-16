using System;
using System.Threading;
using Unity.Mathematics;

namespace TerrainGenerator 
{
    public static class TerrainGenerator
    {
        private static TerrainHandler terrainHandler;
        private static TerrainGeneratorData terrainGeneratorData;

        public static void Init(TerrainHandler terrainHandler)
        {
            TerrainGenerator.terrainHandler = terrainHandler;
            terrainGeneratorData = terrainHandler.terrainGeneratorData;
        }

        public static void GenerateHeightMap()
        {
            int resolution = terrainHandler.terrainGridData.terrain[0].terrainData.heightmapResolution;

            int i = 0;
            int count = terrainHandler.terrainGridData.gridSideCount;

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
                            terrainHandler.terrainGridData.terrain[(int)args[1]].terrainData.SetHeights(0, 0, generatedMap);
                        }, 
                        null);
                    }, 
                    new object[] { tileOffset, i });

                    Thread.Sleep(2);

                    /*
                    generatedMap = getHeightMap(tileOffset);

                    terrainGridHandler.terrainGridData.terrain[i].terrainData.SetHeights(0, 0, generatedMap);
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
            if (terrainGeneratorData.useFalloff)
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
    }
}
