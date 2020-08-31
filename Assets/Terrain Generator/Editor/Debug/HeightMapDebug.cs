using UnityEngine;
using Unity.Mathematics;

namespace TerrainGenerator.Debug
{
    public class HeightMapDebug : MonoBehaviour
    {
        public static DebugType debugType = DebugType.Noise;

        public static FalloffSizeSource falloffSizeSource = FalloffSizeSource.Resolution;
        public static float falloffSize = 32.0f;
        public static float2 falloffOffset;

        public static Texture2D texture;

        public static void DrawHeightMap(int resolution, TerrainGeneratorData terrainGeneratorData)
        {
            if (falloffSizeSource == FalloffSizeSource.Resolution)
            {
                falloffSize = (float)resolution - 1;
            }

            float[,] generatedMap;

            switch (debugType)
            {
                case DebugType.Noise:
                    if (terrainGeneratorData.noiseData.Validate())
                        generatedMap = NoiseMap.Generate(resolution, terrainGeneratorData.noiseData, float2.zero);
                    else
                        generatedMap = null;
                    break;
                
                case DebugType.Falloff:
                    generatedMap = FalloffMap.Generate(resolution, falloffSize, falloffOffset, terrainGeneratorData.falloffData);
                    break;

                case DebugType.Combined:
                    if (terrainGeneratorData.noiseData.Validate())
                        generatedMap = Combined(resolution, falloffSize, falloffOffset, terrainGeneratorData);
                    else
                        generatedMap = null;
                    break;

                default:
                    return;
            }

            if (generatedMap == null)
                return;

            texture = new Texture2D(resolution, resolution);

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            Color[] colorMap = new Color[resolution * resolution];

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    colorMap[y * resolution + x] = Color.Lerp(Color.black, Color.white, generatedMap[x, y]);

            texture.SetPixels(colorMap);
            texture.Apply();
        }


        private static float[,] Combined(int resolution, float falloffSize, float2 falloffOffset, TerrainGeneratorData terrainGeneratorData)
        {
            float[,] noiseMap = NoiseMap.Generate(resolution, terrainGeneratorData.noiseData, float2.zero);
            float[,] falloffMap = FalloffMap.Generate(resolution, falloffSize, falloffOffset, terrainGeneratorData.falloffData);

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    noiseMap[x, y] *= falloffMap[x, y];

            return noiseMap;
        }
    }

    public enum DebugType
    {
        Noise,
        Falloff,
        Combined
    };

    public enum FalloffSizeSource
    {
        Resolution,
        PropertyField
    };
}
