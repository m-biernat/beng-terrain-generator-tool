﻿using UnityEngine;

namespace TerrainGenerator
{
    [System.Serializable]
    public class FalloffData
    {
        public FalloffType type;

        [Range(0, 1)]
        public float a = 0.5f;
        [Range(0, 1)]
        public float b = 0.85f;

        [Space]
        public bool useFalloff = true;
    }

    public enum FalloffType
    { 
        Rectangular,
        Radial
    };
}
