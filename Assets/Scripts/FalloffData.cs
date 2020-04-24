﻿using UnityEngine;

namespace TerrainGenerator
{
    [System.Serializable]
    public class FalloffData
    {
        public FalloffType type;

        public float sharpness = 3;
        public float scale = 2.2f;

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
