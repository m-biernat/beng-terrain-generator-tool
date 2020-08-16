using UnityEngine;
using UnityEditor;

namespace TerrainGenerator
{
    [CustomEditor(typeof(TerrainHandler))]
    public class TerrainHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TerrainHandler terrainGeneratorHandler = (TerrainHandler)target;

            GUILayout.Space(20);

            if (GUILayout.Button("Create New Terrain Generator Data Asset"))
            {
                terrainGeneratorHandler.CreateNewTerrainGeneratorDataAsset();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Create New Terrain Grid Root Object"))
            {
                TerrainGrid.CreateRootObject(terrainGeneratorHandler.transform, terrainGeneratorHandler.terrainGridData);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Update Grid"))
            {
                TerrainGrid.UpdateGrid(terrainGeneratorHandler.transform, terrainGeneratorHandler.terrainGridData);
            }

            if (GUILayout.Button("Reposition Tiles"))
            {
                TerrainGrid.RepositionTiles(terrainGeneratorHandler.terrainGridData);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Reset All Tiles Except Root"))
            {
                TerrainGrid.ResetTiles(1, terrainGeneratorHandler.terrainGridData);
            }

            if (GUILayout.Button("Reset Grid"))
            {
                TerrainGrid.ResetTiles(0, terrainGeneratorHandler.terrainGridData);
                terrainGeneratorHandler.terrainGridData.gridSideCount = 1;
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Open Terrain Generator Window", GUILayout.Height(40)))
            {
                if (terrainGeneratorHandler.terrainGeneratorData is null)
                {
                    Debug.LogError("Terrain Generator Data is not attached!");
                }
                else if (terrainGeneratorHandler.terrainGridData.terrain.Count == 0)
                {
                    Debug.LogError("There is no Terrain Grid Root Object. Create one to proceed!");
                }
                else
                {
                    TerrainGeneratorWindow.OpenWindow(terrainGeneratorHandler);
                }
            }
        }

        [MenuItem("GameObject/Terrain Handler", false, 11)]
        private static void Init()
        {
            GameObject go = new GameObject("Terrain Handler");
            go.AddComponent<TerrainHandler>();
            go.isStatic = true;
        }
    }
}
