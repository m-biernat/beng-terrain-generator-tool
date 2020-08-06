using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace TerrainGenerator
{
    public class TerrainHandler : MonoBehaviour
    {
        public TerrainData terrainData;

        public void CreateRootObject()
        {
            if (terrainData.terrain.Count > 0 && terrainData.terrain[0])
                return;

            if (!AssetDatabase.IsValidFolder("Assets/Terrain Data"))
                AssetDatabase.CreateFolder("Assets", "Terrain Data");

            string currentSceneName = SceneManager.GetActiveScene().name;

            if (!AssetDatabase.IsValidFolder($"Assets/Terrain Data/{currentSceneName}"))
                AssetDatabase.CreateFolder("Assets/Terrain Data", currentSceneName);

            UnityEngine.TerrainData _terrainData = new UnityEngine.TerrainData();
            GameObject go = Terrain.CreateTerrainGameObject(_terrainData);

            string name = transform.name;

            go.transform.name = $"{name} TerrainTile 0";
            go.transform.parent = transform;

            AssetDatabase.CreateAsset(_terrainData, $"Assets/Terrain Data/{currentSceneName}/{name} TerrainTile 0.asset");

            terrainData.terrain.Add(go.GetComponent<Terrain>());

            RepositionTiles();
        }

        public void UpdateGrid()
        {
            ResetTiles(1);

            string currentSceneName = SceneManager.GetActiveScene().name;

            if (terrainData.gridSideCount > 1)
            {
                int tilesCount = terrainData.gridSideCount * terrainData.gridSideCount;

                string name = transform.name;

                string source = $"Assets/Terrain Data/{currentSceneName}/{name} TerrainTile 0.asset";

                for (int i = 1; i < tilesCount; i++)
                {
                    string terrainDataAsset = $"Assets/Terrain Data/{currentSceneName}/{name} TerrainTile {i}.asset";

                    AssetDatabase.CopyAsset(source, terrainDataAsset);

                    GameObject go = Terrain.CreateTerrainGameObject(AssetDatabase.LoadAssetAtPath<UnityEngine.TerrainData>(terrainDataAsset));

                    go.transform.name = $"{name} TerrainTile {i}";
                    go.transform.parent = transform;

                    terrainData.terrain.Add(go.GetComponent<Terrain>());
                }
            }

            RepositionTiles();
        }

        public void ResetTiles(int startIndex)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            int terrainDataCount = terrainData.terrain.Count;

            if (terrainDataCount > startIndex)
            {
                for (int i = startIndex; i < terrainDataCount; i++)
                {
                    string terrainObjectName = terrainData.terrain[i].transform.name;

                    DestroyImmediate(terrainData.terrain[i].gameObject);
                    AssetDatabase.DeleteAsset($"Assets/Terrain Data/{currentSceneName}/{terrainObjectName}.asset");
                }

                terrainData.terrain.RemoveRange(startIndex, terrainDataCount - startIndex);
            }
        }

        public void RepositionTiles()
        {
            Vector3 rootPosition = terrainData.terrain[0].transform.localPosition;
            float tileSize = terrainData.terrain[0].terrainData.size.x;

            float xOffsetMultiplier = -(1 + 0.5f * (terrainData.gridSideCount - 2));
            float zOffsetMultiplier = 0.5f * (terrainData.gridSideCount - 2);

            rootPosition.x = tileSize * xOffsetMultiplier;
            rootPosition.z = tileSize * zOffsetMultiplier;

            terrainData.terrain[0].transform.localPosition = rootPosition;

            if (terrainData.gridSideCount > 1)
            {
                float xOffset = rootPosition.x + tileSize;
                float zOffset = rootPosition.z;

                int i = 1;

                for (int z = 0; z < terrainData.gridSideCount; z++)
                {
                    for (int x = 0; x < terrainData.gridSideCount; x++)
                    {
                        if (x == 0 && z == 0)
                            continue;

                        Vector3 position = terrainData.terrain[i].transform.localPosition;

                        position.x = xOffset;
                        position.z = zOffset;

                        terrainData.terrain[i].transform.position = position;

                        i++;

                        xOffset += tileSize;
                    }

                    zOffset -= tileSize;
                    xOffset = rootPosition.x;
                }
            }
        }
    }
}
