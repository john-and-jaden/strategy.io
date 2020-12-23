using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float scrollDampTime = 0.5f;
    [SerializeField] private float edgeMoveDampTime = 0.5f;
    [SerializeField] private float scrollSensitivity = 0.1f;
    [SerializeField] private float edgeCamMoveThreshold = 0.1f;
    [SerializeField] private float maxCameraMoveSpeed = 0.1f;
    [SerializeField] private float edgeMoveAcceleration = 0.1f;
    [SerializeField] private bool invertScrolling = true;

    Dampable xEdgeSpeedManager;
    Dampable yEdgeSpeedManager;
    Dampable scrollSpeedManager;

    private float worldHalfWidth;
    private float worldHalfHeight;
    private float maxCameraHalfHeight;
    private float minCameraHalfHeight = 5f;
    private int sideMargins = 10;

    void Start()
    {
        worldHalfWidth = GameManager.GridSystem.GetDimensions().x / 2;
        worldHalfHeight = GameManager.GridSystem.GetDimensions().y / 2;
        maxCameraHalfHeight = worldHalfHeight + sideMargins;

        xEdgeSpeedManager = new Dampable(edgeMoveDampTime, -maxCameraMoveSpeed, maxCameraMoveSpeed);
        yEdgeSpeedManager = new Dampable(edgeMoveDampTime, -maxCameraMoveSpeed, maxCameraMoveSpeed);
        scrollSpeedManager = new Dampable(scrollDampTime);
    }

    void Update()
    {
        // Zoom and move camera accordingly
        float scrollAcceleration = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity * (invertScrolling ? -1 : 1);
        scrollSpeedManager.UpdateSpeed(scrollAcceleration);
        Zoom(scrollSpeedManager.Speed);

        // Move camera using keyboard
        MoveCamera(Input.GetAxis("Horizontal") * maxCameraMoveSpeed, Input.GetAxis("Vertical") * maxCameraMoveSpeed);

        // Move camera using mouse if near screen edge
        MoveCameraUsingMouse();

        ClampCameraPosition();
    }

    private void Zoom(float heightDelta)
    {
        // Clamp input
        float goalCameraHalfHeight = Camera.main.orthographicSize + heightDelta;
        float clampedGoalCameraHalfHeight = Mathf.Clamp(goalCameraHalfHeight, minCameraHalfHeight, maxCameraHalfHeight);
        heightDelta = clampedGoalCameraHalfHeight - Camera.main.orthographicSize;

        // If zooming in, move camera according to desired zoom amount and mouse position
        if (heightDelta < 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float changeRatio = heightDelta / Camera.main.orthographicSize;
            Vector2 cameraDelta = changeRatio * (mousePosition - transform.position) * -1;
            MoveCamera(cameraDelta.x, cameraDelta.y);
        }

        // Zoom camera
        Camera.main.orthographicSize += heightDelta;
    }

    private void MoveCameraUsingMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        int xDir = mousePos.x < edgeCamMoveThreshold ? -1 : (mousePos.x > 1 - edgeCamMoveThreshold ? 1 : 0);
        int yDir = mousePos.y < edgeCamMoveThreshold ? -1 : (mousePos.y > 1 - edgeCamMoveThreshold ? 1 : 0);

        xEdgeSpeedManager.UpdateSpeed(edgeMoveAcceleration * xDir);
        yEdgeSpeedManager.UpdateSpeed(edgeMoveAcceleration * yDir);

        MoveCamera(xEdgeSpeedManager.Speed, yEdgeSpeedManager.Speed);
    }

    private void MoveCamera(float xAddition, float yAddition)
    {
        transform.position = new Vector3(transform.position.x + xAddition, transform.position.y + yAddition, transform.position.z);
    }

    private void ClampCameraPosition()
    {
        float cameraHalfHeight = Camera.main.orthographicSize;
        float minCameraHalfWidth = minCameraHalfHeight * Camera.main.aspect;
        float maxZoomLevel = maxCameraHalfHeight - minCameraHalfHeight;
        float zoomLevel = -(Camera.main.orthographicSize - maxCameraHalfHeight);
        float overallMaxDistFromCenterX = worldHalfWidth - minCameraHalfWidth / 2 + sideMargins / 2;
        float currentMaxDistFromCenterX = (zoomLevel / maxZoomLevel) * overallMaxDistFromCenterX;

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -currentMaxDistFromCenterX, currentMaxDistFromCenterX), Mathf.Clamp(transform.position.y, -worldHalfHeight + cameraHalfHeight - sideMargins, worldHalfHeight - cameraHalfHeight + sideMargins), transform.position.z);
    }
}