using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Scrolling Settings")]
    [SerializeField] private float scrollDampTime = 0.5f;
    [SerializeField] private float scrollSensitivity = 0.1f;
    [SerializeField] private float minCameraSize = 5f;
    [SerializeField] private bool invertScrolling = true;

    [Header("Panning Settings")]
    [SerializeField] private float panDampTime = 0.5f;
    [SerializeField] private float maxCameraPanSpeed = 20f;
    [SerializeField] private float panAcceleration = 0.1f;
    [SerializeField] private int panSideMargins = 10;

    [Tooltip("The proportion of the screen which detects mouse input to pan the camera")]
    [SerializeField] private float panScreenEdgeProportion = 0.1f;

    Dampable xEdgeSpeedManager;
    Dampable yEdgeSpeedManager;
    Dampable scrollSpeedManager;

    private float worldSizeX;
    private float worldSize;
    private float maxCameraSize;

    void Start()
    {
        // Set class fields
        worldSizeX = GameManager.GridSystem.GetDimensions().x / 2;
        worldSize = GameManager.GridSystem.GetDimensions().y / 2;
        maxCameraSize = worldSize + panSideMargins;
        maxCameraPanSpeed = maxCameraPanSpeed * Time.deltaTime;
        xEdgeSpeedManager = new Dampable(panDampTime, -maxCameraPanSpeed, maxCameraPanSpeed);
        yEdgeSpeedManager = new Dampable(panDampTime, -maxCameraPanSpeed, maxCameraPanSpeed);
        scrollSpeedManager = new Dampable(scrollDampTime);
    }

    void Update()
    {
        // Zoom and move camera accordingly
        float scrollAcceleration = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity * (invertScrolling ? 1 : -1);
        scrollSpeedManager.UpdateSpeed(scrollAcceleration);
        Zoom(scrollSpeedManager.Speed);

        // Move camera using keyboard
        MoveCamera(Input.GetAxis("Horizontal") * maxCameraPanSpeed, Input.GetAxis("Vertical") * maxCameraPanSpeed);

        // Move camera using mouse if near screen edge
        MoveCameraUsingMouse();

        ClampCameraPosition();
    }

    private void Zoom(float sizeDelta)
    {
        // Clamp input
        float currentCameraSize = Camera.main.orthographicSize;
        float targetCameraSize = currentCameraSize + sizeDelta;
        float clampedTargetCameraSize = Mathf.Clamp(targetCameraSize, minCameraSize, maxCameraSize);
        float clampedSizeDelta = clampedTargetCameraSize - currentCameraSize;

        // If zooming in, move camera according to desired zoom amount and mouse position
        if (clampedSizeDelta < 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float changeRatio = clampedSizeDelta / Camera.main.orthographicSize;
            Vector2 cameraDelta = changeRatio * (mousePosition - transform.position) * -1;
            MoveCamera(cameraDelta.x, cameraDelta.y);
        }

        // Zoom camera
        Camera.main.orthographicSize += clampedSizeDelta;
    }

    private void MoveCameraUsingMouse()
    {
        Vector2 mousePosViewPort = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        int xDir = mousePosViewPort.x < panScreenEdgeProportion ? -1 : (mousePosViewPort.x > 1 - panScreenEdgeProportion ? 1 : 0);
        int yDir = mousePosViewPort.y < panScreenEdgeProportion ? -1 : (mousePosViewPort.y > 1 - panScreenEdgeProportion ? 1 : 0);

        xEdgeSpeedManager.UpdateSpeed(panAcceleration * xDir);
        yEdgeSpeedManager.UpdateSpeed(panAcceleration * yDir);

        MoveCamera(xEdgeSpeedManager.Speed, yEdgeSpeedManager.Speed);
    }

    private void MoveCamera(float xAddition, float yAddition)
    {
        transform.position += new Vector3(xAddition, yAddition, 0);
    }

    private void ClampCameraPosition()
    {
        float cameraSize = Camera.main.orthographicSize;
        float minCameraSizeX = minCameraSize * Camera.main.aspect;
        float maxZoomLevel = maxCameraSize - minCameraSize;
        float zoomLevel = maxCameraSize - cameraSize;
        float overallMaxDistFromCenterX = worldSizeX - minCameraSizeX + panSideMargins;
        float currentMaxDistFromCenterX = (zoomLevel / maxZoomLevel) * overallMaxDistFromCenterX;

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -currentMaxDistFromCenterX, currentMaxDistFromCenterX), Mathf.Clamp(transform.position.y, -worldSize + cameraSize - panSideMargins, worldSize - cameraSize + panSideMargins), transform.position.z);
    }
}
