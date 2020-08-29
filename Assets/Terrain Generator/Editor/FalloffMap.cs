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

            Falloff.RectangularData rectangularData = new Falloff.RectangularData(resolution, size, float2.zero);

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    falloff[x, y] = Falloff.GetRectangular(x, y, size, falloffData, rectangularData);

            return falloff;
        }

        private static float[,] GenerateRadial(int resolution, float size, FalloffData falloffData)
        {
            float[,] falloff = new float[resolution, resolution];

            Falloff.RadialData radialData = new Falloff.RadialData(resolution, size, float2.zero);

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    falloff[x, y] = Falloff.GetRadial(x, y, size, falloffData, radialData);

            return falloff;
        }
    }
}
