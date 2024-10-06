using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public Transform cameraTransform;

    private float currentYaw = 0f;
    public GameObject pauseMenu;

    void Update()
    {
        if (pauseMenu.activeSelf) { return; }
        currentYaw = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(Vector3.up, currentYaw);
    }
}
