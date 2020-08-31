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

        public bool Validate(int layerCount)
        {
            List<string> errors = new List<string>();

            if (splatMapLayers.Count != layerCount)
                errors.Add("Layer count have to match the number in Terrain object");

            if (errors.Count > 0)
            {
                UnityEngine.Debug.LogError($"[SplatMapData] {errors.Count} error(s) found!");
                errors.ForEach((msg) => UnityEngine.Debug.LogWarning(msg));
                return false;
            }
            else
                return true;
        }
    }

    [System.Serializable]
    public class SplatMapLayer
    {
        public AnimationCurve heightCurve;
        [Range(0, 1)]
        public float curveWeight = 1;
    }
}
