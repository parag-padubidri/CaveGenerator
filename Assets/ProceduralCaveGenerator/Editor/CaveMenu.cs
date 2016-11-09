/*
 * ==================================================================
 *  CaveMenu.cs
 *  CaveGenerator
 *
 *  Created by Parag Padubidri
 *  Copyright (c) 2016, Parag Padubidri. All rights reserved.
 * ==================================================================
 */


#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CaveMenu
{
    //Create Custome Editor Menu
    [MenuItem("Cave Generator/Launch Editor")]
    public static void InitCaveEditor()
    {
        CaveGeneratorWindow.InitCaveEditorWindow();
    }
}
