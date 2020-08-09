using JetBrains.Annotations;
using UnityEngine;

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

        public static void Generate()
        {
            float[,] generatedMap;

            int resolution = terrainGridHandler.terrainGridData.terrain[0].terrainData.heightmapResolution;

            //generatedMap = NoiseMap.Generate(resolution, terrainGeneratorData.noiseData);
            generatedMap = Combined(resolution, terrainGeneratorData.noiseData, terrainGeneratorData.falloffData);

            terrainGridHandler.terrainGridData.terrain[0].terrainData.SetHeights(0, 0, generatedMap);
        }

        private static float[,] Combined(int resolution, NoiseData noiseData, FalloffData falloffData)
        {
            float[,] noiseMap = NoiseMap.Generate(resolution, noiseData);
            float[,] falloffMap = FalloffMap.Generate(resolution, falloffData);

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    noiseMap[x, y] *= falloffMap[x, y];

            return noiseMap;
        }
    }
}
