using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour
{
    public float sensitivity;
    Vector2 rotationXY;

    GameObject character;

    // Use this for initialization
    void Start()
    {
        character = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X") * sensitivity, Input.GetAxis("Mouse Y") * sensitivity);
        rotationXY += mouseDelta;

        transform.localRotation = Quaternion.AngleAxis(-rotationXY.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(rotationXY.x, character.transform.up);
    }
}
