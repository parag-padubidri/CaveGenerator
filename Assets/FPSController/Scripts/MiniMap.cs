/*
 * ==================================================================
 *  MiniMap.cs
 *  CaveGenerator
 *
 *  Created by Parag Padubidri
 *  Copyright (c) 2016, Parag Padubidri. All rights reserved.
 * ==================================================================
 */


using UnityEngine;

public class MiniMap : MonoBehaviour
{
    void Start()
    {
        //Reset Minimap size on start 
        if (GameObject.Find("Ground") != null)
        {
            gameObject.GetComponent<Camera>().orthographicSize = GameObject.Find("Ground").transform.localScale.x * 5.5f; //Expensive - Only for Testing
        }
    }
}
