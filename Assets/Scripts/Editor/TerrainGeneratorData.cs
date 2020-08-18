using UnityEngine;

namespace TerrainGenerator
{
    [CreateAssetMenu(fileName = "New Terrain Generator Data", menuName = "Terrain Generator/Terrain Generator Data")]
    public class TerrainGeneratorData : ScriptableObject
    {
        public NoiseData noiseData;

        [Space]
        public FalloffData falloffData;
    }
}
