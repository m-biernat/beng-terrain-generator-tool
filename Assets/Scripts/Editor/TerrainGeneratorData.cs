using UnityEngine;

namespace TerrainGenerator
{
    public class TerrainGeneratorData : ScriptableObject
    {
        public NoiseData noiseData;

        [Space]
        public FalloffData falloffData;
    }
}
