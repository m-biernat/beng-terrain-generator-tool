using UnityEngine;
using UnityEditor;

namespace TerrainGenerator
{
    [CustomEditor(typeof(TerrainGridData))]
    public class TerrainGridDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TerrainGridData terrainGridData = (TerrainGridData)target;

            GUILayout.Space(20);

            if (GUILayout.Button("Create New Terrain Grid Root Object"))
            {
                TerrainGrid.CreateRootObject(terrainGridData.transform, terrainGridData);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Update Grid"))
            {
                TerrainGrid.UpdateGrid(terrainGridData.transform, terrainGridData);
            }

            if (GUILayout.Button("Reposition Tiles"))
            {
                TerrainGrid.RepositionTiles(terrainGridData);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Reset All Tiles Except Root"))
            {
                TerrainGrid.ResetTiles(1, terrainGridData);
            }

            if (GUILayout.Button("Reset Grid"))
            {
                TerrainGrid.ResetTiles(0, terrainGridData);
                terrainGridData.gridSideCount = 1;
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Open Terrain Generator Window", GUILayout.Height(40)))
            {
                if (terrainGridData.terrain.Count == 0)
                    UnityEngine.Debug.LogError("There is no Terrain Grid Root Object. Create one to proceed!");
                else
                    TerrainGeneratorWindow.OpenWindow(terrainGridData);
            }
        }

        [MenuItem("GameObject/Terrain Generator/Terrain Grid", false, 0)]
        private static void Create()
        {
            GameObject go = new GameObject("Terrain Grid");
            go.AddComponent<TerrainGridData>();
            go.isStatic = true;
        }
    }
}
