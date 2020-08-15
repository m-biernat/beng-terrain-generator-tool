using UnityEngine;

namespace TerrainGenerator
{
    public class TerrainGeneratorData : ScriptableObject
    {
        public NoiseData noiseData;

        [Space(20.0f)]
        public bool useFalloff = true;

        [Space]
        public FalloffData falloffData;
    }
}
