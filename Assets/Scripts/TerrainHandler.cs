using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerrainGenerator
{
    public class TerrainHandler : MonoBehaviour
    {
        #if UNITY_EDITOR
        public TerrainGeneratorData terrainGeneratorData;

        public TerrainGridData terrainGridData;

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
        #endif
    }
}
