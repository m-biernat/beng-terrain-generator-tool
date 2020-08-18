using UnityEngine;

namespace TerrainGenerator.Debug
{
    public class HeightMapDebug : MonoBehaviour
    {
        [Space]
        public FalloffSizeSource falloffSizeSource;
        public float falloffSize = 32.0f;

        public static Texture2D texture;

        public static void DrawHeightMap(int resolution, DebugType debugType, TerrainGeneratorData terrainGeneratorData)
        {
            /*
            if (falloffSizeSource == FalloffSizeSource.Resolution)
            {
                falloffSize = (float)resolution - 1;
            }
            */
            float falloffSize = (float)resolution - 1;

            float[,] generatedMap;

            switch (debugType)
            {
                case DebugType.Noise:
                    generatedMap = NoiseMap.Generate(resolution, terrainGeneratorData.noiseData, Unity.Mathematics.float2.zero);
                    break;
                
                case DebugType.Falloff:
                    generatedMap = FalloffMap.Generate(resolution, falloffSize, terrainGeneratorData.falloffData);
                    break;

                case DebugType.Combined:
                    generatedMap = Combined(resolution, falloffSize, terrainGeneratorData);
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


        private static float[,] Combined(int resolution, float falloffSize, TerrainGeneratorData terrainGeneratorData)
        {
            float[,] noiseMap = NoiseMap.Generate(resolution, terrainGeneratorData.noiseData, Unity.Mathematics.float2.zero);
            float[,] falloffMap = FalloffMap.Generate(resolution, falloffSize, terrainGeneratorData.falloffData);

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
