using Unity.Mathematics;

namespace TerrainGenerator 
{
    public static class FalloffMap
    {
        public static float[,] Generate(int resolution, float size, float2 offset, FalloffData falloffData)
        {
            switch (falloffData.type)
            {
                case FalloffType.Rectangular:
                    return GenerateRectangular(resolution, size, offset, falloffData);
                case FalloffType.Radial:
                    return GenerateRadial(resolution, size, offset, falloffData);
                default:
                    return null;
            }
        }

        private static float[,] GenerateRectangular(int resolution, float size, float2 offset, FalloffData falloffData)
        {
            float[,] falloff = new float[resolution, resolution];

            Falloff.RectangularData rectangularData = new Falloff.RectangularData(resolution, size, offset);

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    falloff[x, y] = Falloff.GetRectangular(x, y, size, falloffData, rectangularData);

            return falloff;
        }

        private static float[,] GenerateRadial(int resolution, float size, float2 offset, FalloffData falloffData)
        {
            float[,] falloff = new float[resolution, resolution];

            Falloff.RadialData radialData = new Falloff.RadialData(resolution, size, offset);

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    falloff[x, y] = Falloff.GetRadial(x, y, size, falloffData, radialData);

            return falloff;
        }
    }
}
