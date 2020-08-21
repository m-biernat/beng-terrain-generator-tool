using UnityEngine;
using System.Collections.Generic;

namespace TerrainGenerator
{
    [System.Serializable]
    public class SplatMapData
    {
        [Range(0, 1)]
        public float heightScaleModifier = 1;

        [Space]
        public List<SplatMapLayer> splatMapLayers;
    }

    [System.Serializable]
    public class SplatMapLayer
    {
        //public SplatWeightType splatWeightType;

        public AnimationCurve heightCurve;
        [Range(0, 1)]
        public float curveWeight = 1;
    }

    public enum SplatWeightType
    {
        Curve,
        Steepness
    }
}
