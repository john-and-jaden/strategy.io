using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private int extraSpace = 10;
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

    private float halfWidth;
    private float halfHeight;
    private float topUIHeight = 5;

    void Start()
    {
        halfWidth = GameManager.GridSystem.GetDimensions().x / 2;
        halfHeight = GameManager.GridSystem.GetDimensions().y / 2;

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
        float cameraHeight = Camera.main.orthographicSize;
        float goalCameraHeight = cameraHeight + heightDelta;
        float clampedGoalCameraHeight = Mathf.Clamp(goalCameraHeight, minZoom, halfHeight + extraSpace + topUIHeight);
        heightDelta = clampedGoalCameraHeight - cameraHeight;

        // If zooming in, move camera according to desired zoom amount and mouse position
        if (heightDelta < 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float changeRatio = heightDelta / cameraHeight;
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
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        if (cameraWidth < 2 * halfWidth + 2 * extraSpace)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -halfWidth + cameraWidth / 2 - extraSpace, halfWidth - cameraWidth / 2 + extraSpace), transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
        if (cameraHeight < 2 * halfHeight + 2 * extraSpace)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -halfHeight + cameraHeight / 2 - extraSpace, halfHeight - cameraHeight / 2 + extraSpace + topUIHeight), transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, topUIHeight, transform.position.z);
        }
    }
}