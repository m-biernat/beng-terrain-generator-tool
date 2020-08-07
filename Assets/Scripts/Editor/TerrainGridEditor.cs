using UnityEditor;
using UnityEngine;

namespace TerrainGenerator
{
    [CustomEditor(typeof(TerrainGridHandler))]
    public class TerrainGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TerrainGridHandler terrainHandler = (TerrainGridHandler)target;

            GUILayout.Space(20);

            if (GUILayout.Button("Create Root Object"))
            {
                terrainHandler.CreateRootObject();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Update Grid"))
            {
                terrainHandler.UpdateGrid();
            }

            if (GUILayout.Button("Reposition Tiles"))
            {
                terrainHandler.RepositionTiles();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Reset All Tiles Except Root"))
            {
                terrainHandler.ResetTiles(1);
            }

            if (GUILayout.Button("Reset Grid"))
            {
                terrainHandler.ResetTiles(0);
                terrainHandler.terrainGridData.gridSideCount = 1;
            }
        }
    }
}
