using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Scrolling Settings")]
    [SerializeField] private float scrollDampTime = 0.5f;
    [SerializeField] private float scrollSensitivity = 0.1f;
    [SerializeField] private float minCameraSize = 5f;
    [SerializeField] private bool invertScrolling = false;

    [Header("Panning Settings")]
    [SerializeField] private float panDampTime = 0.5f;
    [SerializeField] private float perSecondMaxPanSpeed = 20f;
    [SerializeField] private float panAcceleration = 0.1f;
    [SerializeField] private int panSideMargin = 10;

    [Tooltip("The proportion of the screen which detects mouse input to pan the camera")]
    [SerializeField] private float panScreenEdgeProportion = 0.1f;

    Dampable xEdgeSpeedManager;
    Dampable yEdgeSpeedManager;
    Dampable scrollSpeedManager;

    private float worldWidth;
    private float worldHeight;
    private float maxCameraSize;
    private float perFrameMaxPanSpeed;

    void Start()
    {
        // Set class fields
        worldWidth = GameManager.GridSystem.GetDimensions().x;
        worldHeight = GameManager.GridSystem.GetDimensions().y;
        maxCameraSize = worldHeight / 2 + panSideMargin;
        xEdgeSpeedManager = new Dampable(panDampTime);
        yEdgeSpeedManager = new Dampable(panDampTime);
        scrollSpeedManager = new Dampable(scrollDampTime);
    }

    void Update()
    {
        // Update maxPanSpeed and related variables
        perFrameMaxPanSpeed = perSecondMaxPanSpeed * Time.deltaTime;
        xEdgeSpeedManager.MinSpeed = yEdgeSpeedManager.MinSpeed = -perFrameMaxPanSpeed;
        xEdgeSpeedManager.MaxSpeed = yEdgeSpeedManager.MaxSpeed = perFrameMaxPanSpeed;

        // Zoom and pan accordingly
        float scrollAcceleration = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity * (invertScrolling ? 1 : -1);
        scrollSpeedManager.UpdateSpeed(scrollAcceleration);
        Zoom(scrollSpeedManager.Speed);

        PanUsingMouseAndKeyboard();

        ClampCameraPosition();
    }

    private void PanUsingMouseAndKeyboard()
    {
        // Pass mouse and keyboard pan inputs to pan method
        Vector2 keyboardPanInput = new Vector2(Input.GetAxis("Horizontal") * perFrameMaxPanSpeed, Input.GetAxis("Vertical") * perFrameMaxPanSpeed);
        Vector2 sumMouseAndKeyboardInputs = GetMousePanInput() + keyboardPanInput;
        Pan(sumMouseAndKeyboardInputs.x, sumMouseAndKeyboardInputs.y);
    }

    private Vector2 GetMousePanInput()
    {
        Vector2 mousePosViewPort = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        int xDir = mousePosViewPort.x < panScreenEdgeProportion ? -1 : (mousePosViewPort.x > 1 - panScreenEdgeProportion ? 1 : 0);
        int yDir = mousePosViewPort.y < panScreenEdgeProportion ? -1 : (mousePosViewPort.y > 1 - panScreenEdgeProportion ? 1 : 0);

        xEdgeSpeedManager.UpdateSpeed(panAcceleration * xDir);
        yEdgeSpeedManager.UpdateSpeed(panAcceleration * yDir);

        return new Vector2(xEdgeSpeedManager.Speed, yEdgeSpeedManager.Speed);
    }

    private void Zoom(float sizeDelta)
    {
        // Clamp input
        float currentCameraSize = Camera.main.orthographicSize;
        float targetCameraSize = currentCameraSize + sizeDelta;
        float clampedTargetCameraSize = Mathf.Clamp(targetCameraSize, minCameraSize, maxCameraSize);
        float clampedSizeDelta = clampedTargetCameraSize - currentCameraSize;

        // If zooming in, pan according to desired zoom amount and mouse position
        if (clampedSizeDelta < 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float changeRatio = clampedSizeDelta / Camera.main.orthographicSize;
            Vector2 cameraDelta = changeRatio * (mousePosition - transform.position) * -1;
            UnclampedPan(cameraDelta.x, cameraDelta.y);
        }

        // Zoom camera
        Camera.main.orthographicSize += clampedSizeDelta;
    }

    private void Pan(float xDelta, float yDelta)
    {
        transform.position += new Vector3(Mathf.Clamp(xDelta, -perFrameMaxPanSpeed, perFrameMaxPanSpeed), Mathf.Clamp(yDelta, -perFrameMaxPanSpeed, perFrameMaxPanSpeed), 0);
    }

    private void UnclampedPan(float xDelta, float yDelta)
    {
        transform.position += new Vector3(xDelta, yDelta, 0);
    }

    private void ClampCameraPosition()
    {
        // Calculate y bounds
        float cameraSize = Camera.main.orthographicSize;
        float boundY = worldHeight / 2 - cameraSize + panSideMargin;
        // Calculate zoom level
        float maxZoomLevel = maxCameraSize - minCameraSize;
        float zoomLevel = maxCameraSize - cameraSize;
        // Calculate x bounds based on zoom level
        float minCameraSizeX = minCameraSize * Camera.main.aspect;
        float maxBoundX = worldWidth / 2 - minCameraSizeX + panSideMargin;
        float boundX = (zoomLevel / maxZoomLevel) * maxBoundX;

        // Calculate clamped position values
        float clampedX = Mathf.Clamp(transform.position.x, -boundX, boundX);
        float clampedY = Mathf.Clamp(transform.position.y, -boundY, boundY);

        // Clamp the camera position
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
