﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;

    [Tooltip("The scroll damp time in seconds")]
    [SerializeField] private float scrollDampTime = 0.5f;

    [SerializeField] private float scrollSensitivity = 0.1f;
    [SerializeField] private float scrollThreshold = 0.1f;
    [SerializeField] private float cameraMoveSpeed = 0.1f;
    [SerializeField] private bool invertScrolling = true;

    private float scrollVelocity = 0f;
    private float lastScrollVelocity = 0f;
    private float scrollDampTimer;
    private Vector2 lastMouseViewportPos = new Vector2(0.5f, 0.5f);
    private Vector2 lastMouseWorldPos;
    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        halfWidth = GameManager.GridSystem.GetDimensions().x / 2;
        halfHeight = GameManager.GridSystem.GetDimensions().y / 2;
    }

    void Update()
    {
        // Get mouse input and update scrollVelocity
        bool isScrolling = Input.GetAxis("Mouse ScrollWheel") != 0;
        scrollVelocity += Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity * (invertScrolling ? -1 : 1);

        if (isScrolling)
        {
            // Update zoom momentum
            lastScrollVelocity = scrollVelocity;
            scrollDampTimer = 0;
        }
        else
        {
            // Update timer
            scrollDampTimer += Time.deltaTime;

            // Dampen scrolling
            scrollVelocity = Mathf.Lerp(lastScrollVelocity, 0, scrollDampTimer / scrollDampTime);
        }

        // Zoom and move camera according to zoom
        Zoom(scrollVelocity);

        // Move camera using keyboard
        transform.position = new Vector3(transform.position.x + Input.GetAxis("Horizontal") * cameraMoveSpeed, transform.position.y + Input.GetAxis("Vertical") * cameraMoveSpeed, transform.position.z);

        // Clamp camera position
        float clampedX = Mathf.Clamp(transform.position.x, -halfWidth, halfWidth);
        float clampedY = Mathf.Clamp(transform.position.y, -halfHeight, halfHeight);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void Zoom(float heightDelta)
    {
        // Get camera size and perform input clamping
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
            transform.position = new Vector3(transform.position.x + cameraDelta.x, transform.position.y + cameraDelta.y, transform.position.z);
        }

        // Zoom camera
        Camera.main.orthographicSize = Mathf.Clamp(cameraHeight + heightDelta, minZoom, maxZoom);
    }
}