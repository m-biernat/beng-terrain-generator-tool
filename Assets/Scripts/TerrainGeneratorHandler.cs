using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace TerrainGenerator
{
    [RequireComponent(typeof(TerrainGridHandler))]
    public class TerrainGeneratorHandler : MonoBehaviour
    {
        public TerrainGridHandler terrainGridHandler;
        public TerrainGeneratorData terrainGeneratorData;

        public void AttachTerrainGridHandler()
        {
            terrainGridHandler = GetComponent<TerrainGridHandler>();
        }

        public void CreateNewTerrainGeneratorDataAsset()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Terrain Data"))
                AssetDatabase.CreateFolder("Assets", "Terrain Data");

            string currentSceneName = SceneManager.GetActiveScene().name;

            if (!AssetDatabase.IsValidFolder($"Assets/Terrain Data/{currentSceneName}"))
                AssetDatabase.CreateFolder("Assets/Terrain Data", currentSceneName);

            terrainGeneratorData = (TerrainGeneratorData)ScriptableObject.CreateInstance(typeof(TerrainGeneratorData));

            AssetDatabase.CreateAsset(terrainGeneratorData, $"Assets/Terrain Data/{currentSceneName} Terrain Generator Data.asset");
        }
    }
}
