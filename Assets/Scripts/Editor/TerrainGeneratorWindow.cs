using UnityEngine;
using UnityEditor;

public class TerrainGeneratorWindow : EditorWindow
{
    private Toolbar toolbar = Toolbar.General;

    private readonly string[] toolbarOptions =
    {
        "General", "Heightmap", "Erosion", "Texturing", "Detail"
    };

    private bool autoGenerate = false;
    private Generate generate = Generate.Heightmap;

    [MenuItem("Window/Terrain Generator")]
    public static void ShowWindow()
    {
        GetWindow<TerrainGeneratorWindow>("Terrain Generator");
    }

    private void OnGUI()
    {
        toolbar = (Toolbar)GUILayout.Toolbar((int)toolbar, toolbarOptions, GUILayout.MinHeight(25.0f));

        switch (toolbar)
        {
            case Toolbar.General:
                General();
                break;

            case Toolbar.Heightmap:
                Heightmap();
                break;
        }

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[STATUS TEXT PH]", GUILayout.Height(35.0f));
        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        autoGenerate = GUILayout.Toggle(autoGenerate, "Auto generate");
        generate = (Generate)EditorGUILayout.EnumPopup(generate, GUILayout.Width(100.0f));
        GUILayout.EndVertical();

        GUILayout.Space(2.5f);

        if (GUILayout.Button("Generate", GUILayout.Height(35.0f), GUILayout.Width(100.0f)))
        {
            // OnClick
        }

        GUILayout.EndHorizontal();
    }

    private void General()
    {
        GUILayout.Label("General");
    }

    private void Heightmap()
    {
        GUILayout.Label("Heightmap");
    }

    private enum Toolbar
    {
        General,
        Heightmap
    }

    private enum Generate
    {
        TerrainOnly,
        Heightmap,
        Combined
    }
}
