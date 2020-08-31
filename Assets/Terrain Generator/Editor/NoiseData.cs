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
        [Range(0.0f, 1.0f)]
        public float amplitudeModifier = 0.5f;
        [Range(1.0f, 10.0f)]
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

        [System.Serializable]
        public class Octave
        {
            public NoiseType noiseType;
            [Range(0.0f, 1.0f)]
            public float weight = 1.0f;
        }

        public bool Validate()
        {
            List<string> errors = new List<string>();

            if (scale <= 0.0f)
                errors.Add("Scale have to be greater than 0");
            if (seed < 1)
                errors.Add("Seed have to be greater than 0");
            if (octaves.Count == 0)
                errors.Add("There must be at least 1 octave");
            if (octaves.Count > 16)
                errors.Add("Maximum number of octaves is 16");
            if ((heightModifierType == HeightModifierType.Curve || heightModifierType == HeightModifierType.Both)
                && curveHeightModifier.length == 0)
                errors.Add("Curve Height Modifier cannot be empty");

            if (errors.Count > 0)
            {
                UnityEngine.Debug.LogError($"[NoiseData] {errors.Count} error(s) found!");
                errors.ForEach((msg) => UnityEngine.Debug.LogWarning(msg));
                return false;
            }    
            else
                return true;
        }
    }

    public enum NoiseType
    {
        Perlin,
        Simplex,
        Cellular,
        Ridged,
        Warped
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