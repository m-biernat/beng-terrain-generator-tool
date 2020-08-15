using System;
using UnityEngine;
using Unity.Mathematics;

namespace TerrainGenerator 
{
    public class TerrainGenerator : MonoBehaviour
    {
        private static TerrainGridHandler terrainGridHandler;
        private static TerrainGeneratorData terrainGeneratorData;

        public static void Init(TerrainGridHandler terrainGridHandler, TerrainGeneratorData terrainGeneratorData)
        {
            TerrainGenerator.terrainGridHandler = terrainGridHandler;
            TerrainGenerator.terrainGeneratorData = terrainGeneratorData;
        }

        public static void GenerateHeightMap()
        {
            float[,] generatedMap;

            int resolution = terrainGridHandler.terrainGridData.terrain[0].terrainData.heightmapResolution;

            int i = 0;
            int count = terrainGridHandler.terrainGridData.gridSideCount;

            float size = ((float)resolution - 1) * count;

            int falloffDiff = resolution - 1;
            float falloffBase = (float)falloffDiff / 2;
            float tileOffsetValue = (count - 1) * falloffBase;

            float2 noiseOffset = terrainGeneratorData.noiseData.offset;
            float2 falloffOffset = terrainGeneratorData.falloffData.offset;
            float2 tileOffset;

            if (count == 1)
                tileOffset = new float2(0, 0);
            else if (count == 2)
                tileOffset = new float2(falloffBase, -falloffBase);
            else
                tileOffset = new float2(tileOffsetValue, -tileOffsetValue);

            Func<float[,]> getHeightMap = 
                GetHeightMapFunc(resolution, size, terrainGeneratorData.noiseData, terrainGeneratorData.falloffData);

            for (int y = 0; y < count; y++)
            {
                for (int x = 0; x < count; x++)
                {
                    terrainGeneratorData.noiseData.offset = noiseOffset + tileOffset;
                    terrainGeneratorData.falloffData.offset = falloffOffset + tileOffset;

                    generatedMap = getHeightMap();

                    terrainGridHandler.terrainGridData.terrain[i].terrainData.SetHeights(0, 0, generatedMap);

                    tileOffset.y += falloffDiff;
                    i++;
                }
                tileOffset.y = -tileOffsetValue;
                tileOffset.x -= falloffDiff;
            }

            terrainGeneratorData.noiseData.offset = noiseOffset;
            terrainGeneratorData.falloffData.offset = falloffOffset;
        }

        private static Func<float[,]> GetHeightMapFunc(int resolution, float size, NoiseData noiseData, FalloffData falloffData)
        {
            if (terrainGeneratorData.useFalloff)
            {
                return () =>
                {
                    float[,] noiseMap = NoiseMap.Generate(resolution, noiseData);

                    Func<int, int, float> getFalloff;

                    if (falloffData.type == FalloffType.Rectangular)
                    {
                        Falloff.RectangularData rectangularData =
                        new Falloff.RectangularData(resolution, size, falloffData);

                        getFalloff = (x, y) => Falloff.GetRectangular(x, y, size, falloffData, rectangularData);
                    }
                    else
                    {
                        Falloff.RadialData radialData =
                        new Falloff.RadialData(resolution, size, falloffData);

                        getFalloff = (x, y) => Falloff.GetRadial(x, y, size, falloffData, radialData);
                    }

                    for (int y = 0; y < resolution; y++)
                        for (int x = 0; x < resolution; x++)
                            noiseMap[x, y] *= getFalloff(x, y);

                    return noiseMap;
                };
            }
            else
                return () => NoiseMap.Generate(resolution, noiseData);
        }
    }
}
