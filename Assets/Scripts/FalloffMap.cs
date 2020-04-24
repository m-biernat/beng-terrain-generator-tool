using Unity.Mathematics;

namespace TerrainGenerator 
{ 
    public static class FalloffMap
    {
        public static float[,] Generate(int resolution, FalloffData falloffData)
        {
            if (!falloffData.Validate())
            {
                UnityEngine.Debug.LogError("Falloff Data is invalid.");
                return null;
            }

            switch (falloffData.type)
            {
                case FalloffType.Rectangular:
                    return GenerateRectangular(resolution, falloffData);
                case FalloffType.Radial:
                    return GenerateRadial(resolution, falloffData);
                default:
                    return null;
            }
        }

        private static float[,] GenerateRectangular(int resolution, FalloffData falloffData)
        {
            float[,] falloff = new float[resolution, resolution];

            float size = (float)resolution - 1;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float2 sample;

                    sample.x = x / size * 2 - 1f;
                    sample.y = y / size * 2 - 1f;

                    float value = 1 - math.max(math.abs(sample.x), math.abs(sample.y));

                    falloff[x, y] = Modifier(value, falloffData.sharpness, falloffData.scale);
                }
            }

            return falloff;
        }

        private static float[,] GenerateRadial(int resolution, FalloffData falloffData)
        {
            float[,] falloff = new float[resolution, resolution];

            float size = (float)resolution - 1;

            float2 center = new float2(size / 2f, size / 2f);

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float distance = math.distance(center, new float2(x, y));

                    float value = 1 - (distance / size);

                    falloff[x, y] = Modifier(value, falloffData.sharpness, falloffData.scale);
                }
            }

            return falloff;
        }

        private static float Modifier(float x, float a, float b)
        {
            return math.pow(x, a) / (math.pow(x, a) + math.pow(b - b * x, a));
        }
    }
}
