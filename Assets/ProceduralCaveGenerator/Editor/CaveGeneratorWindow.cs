/*
 * ==================================================================
 *  CaveGeneratorWindow.cs
 *  CaveGenerator
 *
 *  Created by Parag Padubidri
 *  Copyright (c) 2016, Parag Padubidri. All rights reserved.
 * ==================================================================
 */


using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CaveGeneratorWindow : EditorWindow
{
    public static CaveGeneratorWindow caveWindow;

    public int width;
    public int height;
    public int seed;
    public int threshold;

    public int fillProbability;

    public static void InitCaveEditorWindow()
    {
        //Setup Custom Editor Window
        caveWindow = (CaveGeneratorWindow)EditorWindow.GetWindow<CaveGeneratorWindow>();
        caveWindow.titleContent = new GUIContent("Cave Editor");
        caveWindow.minSize = new Vector2(300, 400);    
    }

    void OnGUI()
    {
        //Create required setup fields
        width = EditorGUILayout.IntField("CaveMap Width  : ", width);
        height = EditorGUILayout.IntField("CaveMap Height : ", height);
        fillProbability = EditorGUILayout.IntSlider("Wall Percent : ", fillProbability, 1, 100, GUILayout.MinWidth(100));

        //Threshold for walls - Autoadjusts to new width & height for reasonable value 
        threshold = EditorGUILayout.IntSlider("Threshold : ", threshold, 1, (width*height)/3, GUILayout.MinWidth(100));

        //Set seed to "0" for random generation or any number other than "0" to be able to regenerate cave next time        
        seed = EditorGUILayout.IntField("Seed : ", seed);

        //Button to Generate Cave
        if (GUILayout.Button("Generate Cave"))
        {
            if (CaveGenerator.cave != null)
            {
                DestroyImmediate(CaveGenerator.cave);
            }

            CaveGenerator.CreateCaveMap(width, height, fillProbability, threshold,seed);
        }

        //Disable buttons by default
        EditorGUI.BeginDisabledGroup(CaveGenerator.cave == null);

        //Button to Save Cave as Prefab with a unique name
        if (GUILayout.Button("Save Generated Cave as Prefab"))
        {
            string prefabPath = "Assets/ProceduralCaveGenerator/Prefabs/";
            Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath + CaveGenerator.cave.name + System.DateTime.Now.ToString("_MMddyyhhmmss") + ".prefab");
            PrefabUtility.ReplacePrefab(CaveGenerator.cave, prefab, ReplacePrefabOptions.ConnectToPrefab);

            //Log output path
            Debug.Log("Saved to " + prefabPath);
        }

        //Button to optionally remove single walls if any
        if (GUILayout.Button("Remove Single Walls"))
        {
            CaveGenerator.CleanCaveMap();
        }

        EditorGUI.EndDisabledGroup();
    }
}
