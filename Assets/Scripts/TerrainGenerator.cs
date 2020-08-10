using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Tilemaps;

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

            //generatedMap = NoiseMap.Generate(resolution, terrainGeneratorData.noiseData);
            //generatedMap = Combined(resolution, terrainGeneratorData.noiseData, terrainGeneratorData.falloffData);

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

            for (int y = 0; y < count; y++)
            {
                for (int x = 0; x < count; x++)
                {
                    terrainGeneratorData.noiseData.offset = noiseOffset + tileOffset;
                    terrainGeneratorData.falloffData.offset = falloffOffset + tileOffset;
                    
                    generatedMap = Combined(resolution, size, terrainGeneratorData.noiseData, terrainGeneratorData.falloffData);
                    
                    terrainGridHandler.terrainGridData.terrain[i].terrainData.SetHeights(0, 0, generatedMap);

                    tileOffset.y += falloffDiff;
                    i++;
                }
                tileOffset.y = -tileOffsetValue;
                tileOffset.x -= falloffDiff;
            }

            terrainGeneratorData.noiseData.offset = noiseOffset;
            terrainGeneratorData.falloffData.offset = falloffOffset;

            //terrainGridHandler.terrainGridData.terrain[0].terrainData.SetHeights(0, 0, generatedMap);
        }

        private static float[,] Combined(int resolution, float size, NoiseData noiseData, FalloffData falloffData)
        {
            float[,] noiseMap = NoiseMap.Generate(resolution, noiseData);
            float[,] falloffMap = FalloffMap.Generate(resolution, size, falloffData);

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    noiseMap[x, y] *= falloffMap[x, y];

            return noiseMap;
        }
    }
}
