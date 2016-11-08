using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CaveMenu
{
    [MenuItem("Cave Generator/Launch Editor")]
    public static void InitCaveEditor()
    {
        CaveGeneratorWindow.InitCaveEditorWindow();
    }

}
