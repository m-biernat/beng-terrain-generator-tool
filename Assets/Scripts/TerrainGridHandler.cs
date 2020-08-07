using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace TerrainGenerator
{
    public class TerrainGridHandler : MonoBehaviour
    {
        public TerrainGridData terrainGridData;

        public void CreateRootObject()
        {
            if (terrainGridData.terrain.Count > 0 && terrainGridData.terrain[0])
                return;

            if (!AssetDatabase.IsValidFolder("Assets/Terrain Data"))
                AssetDatabase.CreateFolder("Assets", "Terrain Data");

            string currentSceneName = SceneManager.GetActiveScene().name;

            if (!AssetDatabase.IsValidFolder($"Assets/Terrain Data/{currentSceneName}"))
                AssetDatabase.CreateFolder("Assets/Terrain Data", currentSceneName);

            TerrainData terrainData = new TerrainData();
            GameObject go = Terrain.CreateTerrainGameObject(terrainData);

            string name = transform.name;

            go.transform.name = $"{name} TerrainTile 0";
            go.transform.parent = transform;

            AssetDatabase.CreateAsset(terrainData, $"Assets/Terrain Data/{currentSceneName}/{name} TerrainTile 0.asset");

            terrainGridData.terrain.Add(go.GetComponent<Terrain>());

            RepositionTiles();
        }

        public void UpdateGrid()
        {
            ResetTiles(1);

            string currentSceneName = SceneManager.GetActiveScene().name;

            if (terrainGridData.gridSideCount > 1)
            {
                int tilesCount = terrainGridData.gridSideCount * terrainGridData.gridSideCount;

                string name = transform.name;

                string source = $"Assets/Terrain Data/{currentSceneName}/{name} TerrainTile 0.asset";

                for (int i = 1; i < tilesCount; i++)
                {
                    string terrainDataAsset = $"Assets/Terrain Data/{currentSceneName}/{name} TerrainTile {i}.asset";

                    AssetDatabase.CopyAsset(source, terrainDataAsset);

                    GameObject go = Terrain.CreateTerrainGameObject(AssetDatabase.LoadAssetAtPath<TerrainData>(terrainDataAsset));

                    go.transform.name = $"{name} TerrainTile {i}";
                    go.transform.parent = transform;

                    terrainGridData.terrain.Add(go.GetComponent<Terrain>());
                }
            }

            RepositionTiles();
        }

        public void ResetTiles(int startIndex)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            int terrainDataCount = terrainGridData.terrain.Count;

            if (terrainDataCount > startIndex)
            {
                for (int i = startIndex; i < terrainDataCount; i++)
                {
                    string terrainObjectName = terrainGridData.terrain[i].transform.name;

                    DestroyImmediate(terrainGridData.terrain[i].gameObject);
                    AssetDatabase.DeleteAsset($"Assets/Terrain Data/{currentSceneName}/{terrainObjectName}.asset");
                }

                terrainGridData.terrain.RemoveRange(startIndex, terrainDataCount - startIndex);
            }
        }

        public void RepositionTiles()
        {
            Vector3 rootPosition = terrainGridData.terrain[0].transform.localPosition;
            float tileSize = terrainGridData.terrain[0].terrainData.size.x;

            float xOffsetMultiplier = -(1 + 0.5f * (terrainGridData.gridSideCount - 2));
            float zOffsetMultiplier = 0.5f * (terrainGridData.gridSideCount - 2);

            rootPosition.x = tileSize * xOffsetMultiplier;
            rootPosition.z = tileSize * zOffsetMultiplier;

            terrainGridData.terrain[0].transform.localPosition = rootPosition;

            if (terrainGridData.gridSideCount > 1)
            {
                float xOffset = rootPosition.x + tileSize;
                float zOffset = rootPosition.z;

                int i = 1;

                for (int z = 0; z < terrainGridData.gridSideCount; z++)
                {
                    for (int x = 0; x < terrainGridData.gridSideCount; x++)
                    {
                        if (x == 0 && z == 0)
                            continue;

                        Vector3 position = terrainGridData.terrain[i].transform.localPosition;

                        position.x = xOffset;
                        position.z = zOffset;

                        terrainGridData.terrain[i].transform.position = position;

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
