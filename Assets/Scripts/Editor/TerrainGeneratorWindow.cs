using UnityEngine;
using UnityEditor;

namespace TerrainGenerator
{
    public class TerrainGeneratorWindow : EditorWindow
    {
        private Toolbar toolbar = Toolbar.General;

        private readonly string[] toolbarOptions = { "General", "Settings" };

        private TerrainGeneratorData terrainGeneratorData;
        private static TerrainGridData terrainGridData;

        private bool utilityFoldout = false;
        private int seed;

        private Vector2 scrollPos;

        [MenuItem("Window/Terrain Generator/Terrain Generator", false, 0)]
        public static void OpenWindow()
        {
            GetWindow<TerrainGeneratorWindow>("Terrain Generator");
        }

        public static void OpenWindow(TerrainGridData terrainGridData)
        {
            GetWindow<TerrainGeneratorWindow>("Terrain Generator");
            TerrainGeneratorWindow.terrainGridData = terrainGridData;
        }

        private void OnGUI()
        {
            toolbar = (Toolbar)GUILayout.Toolbar((int)toolbar, toolbarOptions, GUILayout.MinHeight(25.0f));
            
            GUILayout.Space(5);

            terrainGeneratorData = EditorGUILayout.ObjectField("", terrainGeneratorData, typeof(TerrainGeneratorData), false) as TerrainGeneratorData;

            GUILayout.Space(5);

            switch (toolbar)
            {
                case Toolbar.General:
                    General();
                    break;

                case Toolbar.Settings:
                    Settings();
                    break;
            }
        }

        private enum Toolbar
        {
            General,
            Settings
        }

        private void General()
        {
            if (GUILayout.Button("Create New Terrain Generator Data Asset"))
            {
                CreateNewTerrainGeneratorDataAsset(terrainGeneratorData);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            terrainGridData = EditorGUILayout.ObjectField("", terrainGridData, typeof(TerrainGridData), true) as TerrainGridData;

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (terrainGeneratorData != null && terrainGridData != null)
            {
                EditorGUI.BeginDisabledGroup(TerrainGenerator.isWorking);

                if (GUILayout.Button("Generate\nHeightMap", GUILayout.Height(50), GUILayout.Width(150)))
                {
                    progressBarTitle = "Generating HeightMap";
                    TerrainGenerator.GenerateHeightMap(terrainGridData, terrainGeneratorData);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Generate\nSplatMap", GUILayout.Height(50), GUILayout.Width(150)))
                {
                    progressBarTitle = "Generating SplatMap";
                    TerrainGenerator.GenerateSplatMap(terrainGridData, terrainGeneratorData);
                }

                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (terrainGeneratorData is null)
                    GUILayout.Label("No Terrain Generator Data asset attached!");
                else
                    GUILayout.Label("No Terrain Grid Data object attached!");
            }    

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
        }

        private void Settings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(3);
            GUILayout.BeginVertical();

            utilityFoldout = EditorGUILayout.Foldout(utilityFoldout, "Utility");

            if (utilityFoldout)
            {
                EditorGUILayout.IntField(seed);
                
                if (GUILayout.Button("Generate Random Seed"))
                {
                    seed = Random.Range(0, int.MaxValue);
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (terrainGeneratorData is null)
            {
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.Label("No Terrain Generator Data asset attached!");

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
                return;
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            Editor.CreateEditor(terrainGeneratorData).DrawDefaultInspector();

            EditorGUILayout.EndScrollView();
        }

        public static void CreateNewTerrainGeneratorDataAsset(TerrainGeneratorData terrainGeneratorData)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Terrain Data"))
                AssetDatabase.CreateFolder("Assets", "Terrain Data");

            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            if (!AssetDatabase.IsValidFolder($"Assets/Terrain Data/{currentSceneName}"))
                AssetDatabase.CreateFolder("Assets/Terrain Data", currentSceneName);

            terrainGeneratorData = (TerrainGeneratorData)CreateInstance(typeof(TerrainGeneratorData));

            AssetDatabase.CreateAsset(terrainGeneratorData, $"Assets/Terrain Data/{currentSceneName} Terrain Generator Data.asset");
        }

        #region ProgressBar
        private string progressBarTitle;
        private double timeSinceStartup;

        private void DisplayProgressBar()
        {
            EditorUtility.DisplayProgressBar(progressBarTitle,
                $"Completed {TerrainGenerator.completedTasks} tasks of {TerrainGenerator.scheduledTasks}\t[{ElapsedTime()}]",
                TerrainGenerator.completedTasks / (float)TerrainGenerator.scheduledTasks);

            Repaint();
        }

        private void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
            Repaint();
        }

        private string ElapsedTime()
        {
            double time = EditorApplication.timeSinceStartup - timeSinceStartup;
            return System.TimeSpan.FromSeconds(time).ToString().Remove(12);
        }

        private void OnEnable()
        {
            TerrainGenerator.onStart += OnTerrainGeneratorStart;
            TerrainGenerator.onComplete += OnTerrainGeneratorComplete;
        }

        private void OnDisable()
        {
            TerrainGenerator.onStart -= OnTerrainGeneratorStart;
            TerrainGenerator.onComplete -= OnTerrainGeneratorComplete;
        }

        private void OnTerrainGeneratorStart()
        {
            timeSinceStartup = EditorApplication.timeSinceStartup;
        }

        private void OnTerrainGeneratorComplete()
        {
            UnityEngine.Debug.Log($"Terrain Generator completed \"{progressBarTitle}\" in {ElapsedTime()}");
            ClearProgressBar();
        }

        private void OnInspectorUpdate()
        {
            if (TerrainGenerator.isWorking)
                DisplayProgressBar();
        }
        #endregion
    }
}
