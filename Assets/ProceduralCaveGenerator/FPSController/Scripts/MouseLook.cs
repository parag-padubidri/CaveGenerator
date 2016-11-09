/*
 * ==================================================================
 *  MouseLook.cs
 *  CaveGenerator
 *
 *  Created by Parag Padubidri
 *  Copyright (c) 2016, Parag Padubidri. All rights reserved.
 * ==================================================================
 */


using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity;
    public float minY = -60F;
    public float maxY = 60F;

    Vector2 rotationXY;
    GameObject character;

    void Start()
    {
        character = transform.parent.gameObject;
    }

    void Update()
    {
        //Get mouse input & store cumulative value
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X") * sensitivity, Input.GetAxis("Mouse Y") * sensitivity);
        rotationXY += mouseDelta;

        //Clamp y-rotation angle
        rotationXY.y = ClampAngle(rotationXY.y, minY, maxY);

        //Apply y-roation to camera & x-roation to character
        transform.localRotation = Quaternion.AngleAxis(-rotationXY.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(rotationXY.x, character.transform.up);
    }

    //Rotation Clamping function
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
         angle += 360F;
        if (angle > 360F)
         angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
