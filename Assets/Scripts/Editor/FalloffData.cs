using UnityEngine;
using Unity.Mathematics;

namespace TerrainGenerator
{
    [System.Serializable]
    public class FalloffData
    {
        public FalloffType type;

        public float2 offset;

        public float sharpness = 3;
        public float scale = 2.2f;

        [Space]
        public bool useFalloff = true;

        public bool Validate()
        {
            if (scale <= 0)
                return false;

            return true;
        }
    }

    public enum FalloffType
    { 
        Rectangular,
        Radial
    };
}
