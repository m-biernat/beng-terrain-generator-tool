using UnityEngine;
using UnityEditor;

namespace TerrainGenerator
{
    [CustomEditor(typeof(TerrainGeneratorHandler))]
    public class TerrainGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TerrainGeneratorHandler terrainGeneratorHandler = (TerrainGeneratorHandler)target;

            GUILayout.Space(20);

            if (GUILayout.Button("Attach Terrain Grid Handler"))
            {
                terrainGeneratorHandler.AttachTerrainGridHandler();
            }

            if (GUILayout.Button("Create New Terrain Generator Data Asset"))
            {
                terrainGeneratorHandler.CreateNewTerrainGeneratorDataAsset();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Open Terrain Generator Window", GUILayout.Height(40)))
            {
                if (terrainGeneratorHandler.terrainGridHandler is null)
                {
                    Debug.LogError("Terrain Grid Handler is not attached!");
                }
                else if (terrainGeneratorHandler.terrainGeneratorData is null)
                {
                    Debug.LogError("Terrain Generator Data is not attached!");
                }
                else if (terrainGeneratorHandler.terrainGridHandler.terrainGridData.terrain.Count == 0)
                {
                    Debug.LogError("There is no Terrain Grid Root Object. Create one to proceed!");
                }
                else
                {
                    TerrainGeneratorWindow.OpenWindow(terrainGeneratorHandler.terrainGridHandler, terrainGeneratorHandler.terrainGeneratorData);
                }
            }
        }
    }
}
