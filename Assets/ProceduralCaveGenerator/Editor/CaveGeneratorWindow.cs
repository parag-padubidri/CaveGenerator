using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CaveGeneratorWindow : EditorWindow
{
    public static CaveGeneratorWindow caveWindow;

    public int width;
    public int height;
    public int seed;
    public int threshhold;

    public int fillProbability;

    public static void InitCaveEditorWindow()
    {
        caveWindow = (CaveGeneratorWindow)EditorWindow.GetWindow<CaveGeneratorWindow>();
        GUIContent windowTitle = new GUIContent("Node Editor");
        caveWindow.titleContent = windowTitle;
        caveWindow.minSize = new Vector2(300, 400);    
    }

    void OnGUI()
    {
        width = EditorGUILayout.IntField("CaveMap Width  : ", width);
        height = EditorGUILayout.IntField("CaveMap Height : ", height);
        fillProbability = EditorGUILayout.IntSlider("Wall Percent : ", fillProbability, 1, 100, GUILayout.MinWidth(100));
        threshhold = EditorGUILayout.IntSlider("Threshold : ", threshhold, 1, (width*height)/3, GUILayout.MinWidth(100));
        seed = EditorGUILayout.IntField("Seed : ", seed);

        //Generate Cave
        if (GUILayout.Button("Generate Cave"))
        {
            if (CaveGenerator.cave != null)
            {
                Debug.Log("Destroying");
                DestroyImmediate(CaveGenerator.cave);
            }

            CaveGenerator.CreateCaveMap(width, height, fillProbability, threshhold,seed);
        }

        //Save Cave as Prefab
        EditorGUI.BeginDisabledGroup(CaveGenerator.cave == null);

        if (GUILayout.Button("Save Generated Cave as Prefab"))
        {
            string prefabPath = "Assets/ProceduralCaveGenerator/Prefabs/";
            Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath + CaveGenerator.cave.name + System.DateTime.Now.ToString("_MMddyyhhmmss") + ".prefab");
            PrefabUtility.ReplacePrefab(CaveGenerator.cave, prefab, ReplacePrefabOptions.ConnectToPrefab);
            Debug.Log("Saved to " + prefabPath);
        }

        EditorGUI.EndDisabledGroup();
    }
}
