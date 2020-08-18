using UnityEngine;
using UnityEditor;

namespace TerrainGenerator.Debug
{
    public class HeightMapDebugWindow : EditorWindow
    {
        private TerrainGeneratorData terrainGeneratorData;

        private Resolution resolution = Resolution._33x33;

        private Vector2 scrollPos;

        private bool autoGenerate = false;

        [MenuItem("Window/Terrain Generator/HeightMap Debug", false, 1)]
        public static void OpenWindow()
        {
            GetWindow<HeightMapDebugWindow>("HeightMap Debug");
        }

        private void OnGUI()
        {
            terrainGeneratorData = EditorGUILayout.ObjectField("", terrainGeneratorData, typeof(TerrainGeneratorData), false) as TerrainGeneratorData;

            GUILayout.Space(5);

            if (GUILayout.Button("Create New Terrain Generator Data Asset"))
            {
                TerrainGeneratorWindow.CreateNewTerrainGeneratorDataAsset(terrainGeneratorData);
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            GUILayout.Label("Resolution");
            resolution = (Resolution)EditorGUILayout.EnumPopup(resolution);
            
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (HeightMapDebug.texture is null)
            {
                GUILayout.Space(113);
                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();
                GUILayout.Label("No Preview Available");
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();
                GUILayout.Space(113);
            }
            else
            {
                GUI.DrawTexture(new Rect((position.width - 250) / 2, 95, 250, 250), HeightMapDebug.texture);
                GUILayout.Space(245);

                if (autoGenerate)
                    HeightMapDebug.DrawHeightMap((int)resolution, terrainGeneratorData);
            }
            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (terrainGeneratorData is null)
            {
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.Label("No Terrain Generator Data asset attached!");

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
            }
            else
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                Editor.CreateEditor(terrainGeneratorData).DrawDefaultInspector();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                HeightMapDebug.falloffSizeSource = 
                    (FalloffSizeSource)EditorGUILayout.EnumPopup("Falloff Size Source", HeightMapDebug.falloffSizeSource);
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                HeightMapDebug.falloffSize = EditorGUILayout.FloatField("Falloff Size", HeightMapDebug.falloffSize);
                GUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();
                autoGenerate = GUILayout.Toggle(autoGenerate, "Auto Generate");
                HeightMapDebug.debugType = (DebugType)EditorGUILayout.EnumPopup(HeightMapDebug.debugType, GUILayout.Width(100.0f));
                GUILayout.EndVertical();

                GUILayout.Space(2.5f);

                if (GUILayout.Button("Generate", GUILayout.Height(35.0f), GUILayout.Width(100.0f)))
                {
                    HeightMapDebug.DrawHeightMap((int)resolution, terrainGeneratorData);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
        }

        private enum Resolution
        {
            _33x33 = 33,
            _65x65 = 65,
            _129x129 = 129,
            _257x257 = 257,
            _513x513 = 513,
        }
    }
}
