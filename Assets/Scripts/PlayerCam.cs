using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCam : MonoBehaviour
{
    public float sensY;
    public float sensX;

    public Transform orientation;
    public Transform objectHolder;

    public GameObject inventory;

    float yRotation;
    float xRotation;

    public static bool paused = false;
    public static bool guideOn = false;

    // Start is called before the first frame update
    void Start()
    {
        guideOn = true;
        paused = true;
        inventory.SetActive(true);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !paused && !guideOn)
        {
            inventory.SetActive(true);
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            paused = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && paused && !guideOn)
        {
            inventory.SetActive(false);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            paused = false;
        }
    }

    void LateUpdate()
    {
        if (!paused)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensX;
            float mouseY = Input.GetAxis("Mouse Y") * sensY;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);

            //set yRotation of objectHolder
            objectHolder.rotation = Quaternion.Euler(0, yRotation, 0);

        }
    }
}
