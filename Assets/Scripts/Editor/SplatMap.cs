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

                    coord.x = (float)y / resolution;
                    coord.y = (float)x / resolution;

                    float height = terrainData.GetInterpolatedHeight(coord.x, coord.y) / heightScale;

                    float sum = 0;

                    //float steepness = terrainData.GetSteepness(coord.x, coord.y);

                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                    {
                        splatWeights[i] = GetHeightCurveWeight(splatMapData.splatMapLayers[i], height);

                        //splatWeights[i] = splatMapData.splatMapLayers[i].heightCurve.Evaluate(height) * splatMapData.splatMapLayers[i].curveWeight;
                        //splatWeights[i] += 1.0f - math.clamp(steepness / 20, 0, 1);

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
