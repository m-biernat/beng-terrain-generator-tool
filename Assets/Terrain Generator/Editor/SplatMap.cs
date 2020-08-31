using UnityEngine;
using Unity.Mathematics;

namespace TerrainGenerator
{
    public class SplatMap
    {
        public static float[,,] Generate(TerrainData terrainData, SplatMapData splatMapData)
        {
            int resolution = terrainData.alphamapResolution;
            
            float heightScale = terrainData.heightmapScale.y * splatMapData.heightScaleModifier;
            
            float[,,] splatMap = new float[resolution, resolution, terrainData.alphamapLayers];
            float[] splatWeights = new float[terrainData.alphamapLayers];

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float2 coord;

                    coord.x = (y + 0.5f) / resolution;
                    coord.y = (x + 0.5f) / resolution;

                    float height = terrainData.GetInterpolatedHeight(coord.x, coord.y) / heightScale;

                    float sum = 0;

                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                    {
                        splatWeights[i] = GetHeightCurveWeight(splatMapData.splatMapLayers[i], height);

                        sum += splatWeights[i];
                    }

                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                    {
                        splatWeights[i] /= sum;

                        splatMap[x, y, i] = splatWeights[i];
                    }
                }
            }

            return splatMap;
        }

        private static float GetHeightCurveWeight(SplatMapLayer layer, float height)
        {
            return layer.heightCurve.Evaluate(height) * layer.curveWeight;
        }
    }
}
