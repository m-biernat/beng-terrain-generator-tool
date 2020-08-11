using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

namespace TerrainGenerator
{
    [System.Serializable]
    public class NoiseData
    {
        public float scale = 2.0f;
        public float2 offset;

        [Space]
        public uint seed = uint.MaxValue;

        [Space]
        public List<NoiseType> octaveNoiseType;
        public float amplitudeModifier = 0.5f;
        public float frequencyModifier = 2.0f;

        [Space]
        public ExtremaType extremaType;
        [Range(0.5f, 1.0f)]
        public float extremumNoiseValue = 0.8f;

        public bool Validate()
        {
            if (scale <= 0.0f)
                return false;
            if (seed < 1)
                return false;
            if (octaveNoiseType.Count == 0)
                return false;

            return true;
        }
    }

    public enum NoiseType
    {
        Perlin,
        Simplex,
        Cellular
    };

    public enum ExtremaType
    { 
        Local,
        Global
    };
}