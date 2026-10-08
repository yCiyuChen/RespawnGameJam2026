using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class playerCam : MonoBehaviour
{
    public Slider slider;
    public float mouseSens = 200f;
    public Transform playerBody;

    public Transform orientation;

    float xRotation;
    float yRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mouseSens = PlayerPrefs.GetFloat("currentSensitivity", 100);
        slider.value = mouseSens/10;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetFloat("currentSensitivity", mouseSens);
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSens;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void AdjustSpeed(float newSpeed)
    {
        mouseSens = newSpeed * 10;
    }
}
