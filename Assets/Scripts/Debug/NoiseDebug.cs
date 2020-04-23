using UnityEngine;

namespace TerrainGenerator
{
    public class NoiseDebug : MonoBehaviour
    {
        public Renderer noiseRenderer;
        public string textureName = "_BaseMap";

        [Space]
        public NoiseData noiseData;

        public void DrawNoiseMap()
        {
            float[,] noiseMap = NoiseMap.Generate(noiseData);

            if (noiseMap == null)
                return;

            Texture2D texture = new Texture2D(noiseData.size, noiseData.size);

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            Color[] colorMap = new Color[noiseData.size * noiseData.size];

            for (int y = 0; y < noiseData.size; y++)
                for (int x = 0; x < noiseData.size; x++)
                    colorMap[y * noiseData.size + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);

            texture.SetPixels(colorMap);
            texture.Apply();

            noiseRenderer.sharedMaterial.SetTexture(textureName, texture);
        }

        private void OnValidate()
        {
            DrawNoiseMap();
        }
    }
}
