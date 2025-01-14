using System;
using UnityEngine;

/**
 * From Fusion tutorial https://doc.photonengine.com/fusion/current/tutorials/shared-mode-basics/3-movement-and-camera
 */
public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField, Tooltip("Sensitivity of mouse movement")]
    private float mouseSensitivity = 10f;

    [SerializeField, Tooltip("Maximum angle to clamp vertical rotation")]
    private float clampRotationAngle = 45f;

    private float verticalRotation;
    private float horizontalRotation;

    private Transform target;

    internal void SetTarget(Transform transform)
    {
        this.target = transform;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        verticalRotation -= mouseY * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -clampRotationAngle, clampRotationAngle);

        horizontalRotation += mouseX * mouseSensitivity;
        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }
}
