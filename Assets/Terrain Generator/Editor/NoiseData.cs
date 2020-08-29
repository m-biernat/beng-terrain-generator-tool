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
        public List<Octave> octaves;
        public float amplitudeModifier = 0.5f;
        public float frequencyModifier = 2.0f;

        [Space]
        public ExtremaType extremaType;
        [Range(0.5f, 1.0f)]
        public float extremumNoiseValue = 0.8f;

        [Space]
        public HeightModifierType heightModifierType;
        [Range(0.0f, 1.0f)]
        public float globalHeightModifier = 1.0f;
        public AnimationCurve curveHeightModifier = AnimationCurve.Linear(0, 0, 1, 1);

        public bool Validate()
        {
            if (scale <= 0.0f)
                return false;
            if (seed < 1)
                return false;
            if (octaves.Count == 0)
                return false;
            if ((heightModifierType == HeightModifierType.Curve || heightModifierType == HeightModifierType.Both)
                && curveHeightModifier.length == 0)
                return false;

            return true;
        }

        [System.Serializable]
        public class Octave
        {
            public NoiseType noiseType;
            [Range(0.0f, 1.0f)]
            public float weight = 1.0f;
        }
    }

    public enum NoiseType
    {
        Perlin,
        Simplex,
        Cellular,
        Ridge
    };

    public enum ExtremaType
    { 
        Local,
        Global
    };

    public enum HeightModifierType
    { 
        None,
        Global,
        Curve,
        Both
    };
}