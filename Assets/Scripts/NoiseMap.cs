using Unity.Mathematics;

namespace TerrainGenerator
{
    public static class NoiseMap
    {
        public static float[,] Generate(int resolution, NoiseData noiseData)
        {
            if (!noiseData.Validate())
            {
                UnityEngine.Debug.LogError("Noise Data is invalid.");
                return null;
            }

            float[,] noiseMap = new float[resolution, resolution];

            float halfSize = resolution / 2.0f;

            int octaves = noiseData.octaveNoiseType.Count;
            float2[] octaveOffsets = new float2[octaves];

            Random rand = new Random(noiseData.seed);

            for (int i = 0; i < octaves; i++)
            {
                octaveOffsets[i].x = rand.NextFloat(-10000, 10000) + noiseData.offset.x;
                octaveOffsets[i].y = rand.NextFloat(-10000, 10000) + noiseData.offset.y;
            }

            float maxNoiseValue = float.MinValue;
            float minNoiseValue = float.MaxValue;

            System.Func<float2, float> generateNoise;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseValue = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float2 sample;

                        sample.x = (x - halfSize + octaveOffsets[i].x) / noiseData.scale * frequency;
                        sample.y = (y - halfSize + octaveOffsets[i].y) / noiseData.scale * frequency;

                        generateNoise = GetNoiseFunc(noiseData.octaveNoiseType[i]);
                        noiseValue += generateNoise(sample) * amplitude;

                        amplitude *= noiseData.amplitudeModifier;
                        frequency *= noiseData.frequencyModifier;
                    }

                    if (noiseValue > maxNoiseValue)
                        maxNoiseValue = noiseValue;
                    if (noiseValue < minNoiseValue)
                        minNoiseValue = noiseValue;

                    noiseMap[x, y] = noiseValue;
                }
            }

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    noiseMap[x, y] = math.unlerp(minNoiseValue, maxNoiseValue, noiseMap[x, y]);
            
            return noiseMap;
        }

        private static System.Func<float2, float> GetNoiseFunc(NoiseType noiseType)
        {
            switch (noiseType)
            {
                case NoiseType.Perlin:
                    return noise.cnoise;
                case NoiseType.Simplex:
                    return noise.snoise;
                case NoiseType.Cellular:
                    return (float2 P) => noise.cellular(P).x * 2 - 1;
                default:
                    return null;
            }
        }
    }
}
