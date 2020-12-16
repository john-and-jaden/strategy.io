using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;

    [Tooltip("The scroll damp time in seconds")]
    [SerializeField] private float scrollDampTime = 0.5f;
    [SerializeField] private float edgeMoveDampTime = 0.5f;
    [SerializeField] private float scrollSensitivity = 0.1f;
    [SerializeField] private float edgeCamMoveThreshold = 0.1f;
    [SerializeField] private float maxCameraMoveSpeed = 0.1f;
    [SerializeField] private bool invertScrolling = true;
    [SerializeField] private float edgeMoveAcceleration = 0.1f;

    Dampable edgeMoveXDampable;
    Dampable edgeMoveYDampable;
    Dampable scrollMoveDampable;

    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        halfWidth = GameManager.GridSystem.GetDimensions().x / 2;
        halfHeight = GameManager.GridSystem.GetDimensions().y / 2;

        edgeMoveXDampable = new Dampable(edgeMoveDampTime);
        edgeMoveYDampable = new Dampable(edgeMoveDampTime);
        scrollMoveDampable = new Dampable(scrollDampTime);
    }

    void Update()
    {
        // Get mouse input and update scrollVelocity
        scrollMoveDampable.UpdateAndDampen(Input.GetAxis("Mouse ScrollWheel"), scrollMoveDampable.currentVelocity + Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity * (invertScrolling ? -1 : 1));

        // Zoom and move camera according to zoom
        Zoom(scrollMoveDampable.currentVelocity);

        // Move camera using keyboard
        AddToCurrentCameraPosition(Input.GetAxis("Horizontal") * maxCameraMoveSpeed, Input.GetAxis("Vertical") * maxCameraMoveSpeed);

        // Move camera using mouse if near screen edge
        MoveCameraUsingMouse();

        // Clamp camera position
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -halfWidth, halfWidth), Mathf.Clamp(transform.position.y, -halfHeight, halfHeight), transform.position.z);
    }

    private void Zoom(float heightDelta)
    {
        // Clamp input
        float cameraHeight = Camera.main.orthographicSize;
        float goalCameraHeight = cameraHeight + heightDelta;
        float clampedGoalCameraHeight = Mathf.Clamp(goalCameraHeight, minZoom, maxZoom);
        heightDelta = clampedGoalCameraHeight - cameraHeight;

        // If zooming in, move camera according to desired zoom amount and mouse position
        if (heightDelta < 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float changeRatio = heightDelta / cameraHeight;
            Vector2 cameraDelta = changeRatio * (mousePosition - transform.position) * -1;
            AddToCurrentCameraPosition(cameraDelta.x, cameraDelta.y);
        }

        // Zoom camera
        Camera.main.orthographicSize += heightDelta;
    }

    private void MoveCameraUsingMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        int xFactor = mousePos.x < edgeCamMoveThreshold ? -1 : (mousePos.x > 1 - edgeCamMoveThreshold ? 1 : 0);
        int yFactor = mousePos.y < edgeCamMoveThreshold ? -1 : (mousePos.y > 1 - edgeCamMoveThreshold ? 1 : 0);

        edgeMoveXDampable.UpdateAndDampen(xFactor, Mathf.Clamp(edgeMoveXDampable.currentVelocity + edgeMoveAcceleration * xFactor, -maxCameraMoveSpeed, maxCameraMoveSpeed));
        edgeMoveYDampable.UpdateAndDampen(yFactor, Mathf.Clamp(edgeMoveYDampable.currentVelocity + edgeMoveAcceleration * yFactor, -maxCameraMoveSpeed, maxCameraMoveSpeed));

        AddToCurrentCameraPosition(edgeMoveXDampable.currentVelocity, edgeMoveYDampable.currentVelocity);
    }

    private void AddToCurrentCameraPosition(float xAddition, float yAddition)
    {
        transform.position = new Vector3(transform.position.x + xAddition, transform.position.y + yAddition, transform.position.z);
    }
}