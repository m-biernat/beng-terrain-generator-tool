﻿using UnityEngine;

namespace TerrainGenerator.Hidden
{
    public class HeightmapDebug : MonoBehaviour
    {
        public Renderer noiseRenderer;
        public string textureName = "_BaseMap";

        [Space]
        public int resolution = 33;

        [Space]
        public FalloffSizeSource falloffSizeSource;
        public float falloffSize = 32.0f;

        [Space]
        public NoiseData noiseData;

        [Space]
        public FalloffData falloffData;

        [Space]
        public DebugType debugType;

        public void DrawNoiseMap()
        {
            if (resolution < 3)
            {
                Debug.LogError("Resolution must be greater than 3");
                return;
            }
            
            if (falloffSizeSource == FalloffSizeSource.Resolution)
            {
                falloffSize = (float)resolution - 1;
            }

            float[,] generatedMap;

            switch (debugType)
            {
                case DebugType.Noise:
                    generatedMap = NoiseMap.Generate(resolution, noiseData, Unity.Mathematics.float2.zero);
                    break;
                
                case DebugType.Falloff:
                    generatedMap = FalloffMap.Generate(resolution, falloffSize, falloffData);
                    break;

                case DebugType.Combined:
                    generatedMap = Combined();
                    break;

                default:
                    return;
            }

            if (generatedMap == null)
                return;

            Texture2D texture = new Texture2D(resolution, resolution);

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            Color[] colorMap = new Color[resolution * resolution];

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    colorMap[y * resolution + x] = Color.Lerp(Color.black, Color.white, generatedMap[x, y]);

            texture.SetPixels(colorMap);
            texture.Apply();

            noiseRenderer.sharedMaterial.SetTexture(textureName, texture);
        }

        private void OnValidate()
        {
            DrawNoiseMap();
        }

        private float[,] Combined()
        {
            float[,] noiseMap = NoiseMap.Generate(resolution, noiseData, Unity.Mathematics.float2.zero);
            float[,] falloffMap = FalloffMap.Generate(resolution, falloffSize, falloffData);

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    noiseMap[x, y] *= falloffMap[x, y];

            return noiseMap;
        }
    }

    public enum DebugType
    {
        Noise,
        Falloff,
        Combined
    };

    public enum FalloffSizeSource
    {
        Resolution,
        PropertyField
    };
}