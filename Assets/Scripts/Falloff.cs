using Unity.Mathematics;

namespace TerrainGenerator
{
    public class Falloff
    {
        public struct RectangularData
        {
            public float factor, xOffsetFactor, yOffsetFactor;

            public RectangularData(int resolution, float size, float2 tileOffset)
            {
                factor = 1 / (size / (resolution - 1)) * 0.5f;

                xOffsetFactor = 2.0f * tileOffset.x / (resolution - 1) * factor;
                yOffsetFactor = 2.0f * tileOffset.y / (resolution - 1) * factor;
            }
        };

        public static float GetRectangular(int x, int y, float size, FalloffData falloffData, RectangularData rectangularData)
        {
            float2 sample;

            sample.x = x / size - rectangularData.factor + rectangularData.xOffsetFactor;
            sample.y = y / size - rectangularData.factor + rectangularData.yOffsetFactor;

            float value = 1 - math.max(math.abs(sample.x), math.abs(sample.y));

            return Modify(value, falloffData.sharpness, falloffData.scale);
        }

        public struct RadialData
        {
            private float factor, coord;

            public float2 center;

            public RadialData(int resolution, float size, float2 tileOffset)
            {
                factor = 1 / (size / (resolution - 1));
                coord = factor * size / 2;

                center = new float2(coord - tileOffset.x, coord - tileOffset.y);
            }
        };

        public static float GetRadial(int x, int y, float size, FalloffData falloffData, RadialData radialData)
        {
            float distance = math.distance(radialData.center, new float2(x, y));

            float value = 1 - (distance / size);

            return Modify(value, falloffData.sharpness, falloffData.scale);
        }

        private static float Modify(float x, float a, float b)
        {
            return math.pow(x, a) / (math.pow(x, a) + math.pow(b - b * x, a));
        }
    }
}
