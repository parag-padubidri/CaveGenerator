/*
 * ==================================================================
 *  CharacterController.cs
 *  CaveGenerator
 *
 *  Created by Parag Padubidri
 *  Copyright (c) 2016, Parag Padubidri. All rights reserved.
 * ==================================================================
 */


using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    public int speed = 2;
    float horizontal;
    float vertical;

    void Start()
    {
        //Hide Cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //Get keyboard input axes
        horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        // Simple Translation
        transform.Translate(horizontal,0,vertical);
    }
}
