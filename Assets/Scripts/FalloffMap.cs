using Unity.Mathematics;

namespace TerrainGenerator 
{ 
    public static class FalloffMap
    {
        public static float[,] Generate(int resolution, float size, FalloffData falloffData)
        {
            if (!falloffData.Validate())
            {
                UnityEngine.Debug.LogError("Falloff Data is invalid.");
                return null;
            }

            switch (falloffData.type)
            {
                case FalloffType.Rectangular:
                    return GenerateRectangular(resolution, size, falloffData);
                case FalloffType.Radial:
                    return GenerateRadial(resolution, size, falloffData);
                default:
                    return null;
            }
        }

        private static float[,] GenerateRectangular(int resolution, float size, FalloffData falloffData)
        {
            float[,] falloff = new float[resolution, resolution];

            float factor = 1 / (size / (resolution - 1)) * 0.5f;

            float xOffsetFactor = 2.0f * falloffData.offset.x / (resolution - 1) * factor;
            float yOffsetFactor = 2.0f * falloffData.offset.y / (resolution - 1) * factor;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float2 sample;

                    sample.x = x / size - factor + xOffsetFactor;
                    sample.y = y / size - factor + yOffsetFactor;

                    float value = 1 - math.max(math.abs(sample.x), math.abs(sample.y));

                    falloff[x, y] = Modifier(value, falloffData.sharpness, falloffData.scale);
                }
            }

            return falloff;
        }

        private static float[,] GenerateRadial(int resolution, float size, FalloffData falloffData)
        {
            float[,] falloff = new float[resolution, resolution];

            float factor = 1 / (size / (resolution - 1));
            float coord = factor * size / 2;

            float2 center = new float2(coord - falloffData.offset.x, coord - falloffData.offset.y);

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
