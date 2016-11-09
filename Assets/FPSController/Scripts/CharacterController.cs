using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{
    public int speed = 2;
    float horizontal;
    float vertical;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Simple Translation
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(horizontal,0,vertical);
    }
}
