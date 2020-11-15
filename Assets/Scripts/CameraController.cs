using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minZoom = 5f;
    public float maxZoom = 15f;
    [Tooltip("The scroll damp time in seconds")]
    public float scrollDampTime = 0.5f;
    public float scrollSensitivity = 1f;
    public float scrollThreshold = 0.1f;

    private float scrollVelocity = 0f;
    private float lastScrollVelocity = 0f;
    private float scrollDampTimer;
    private Vector2 lastMouseViewportPos;
    private Vector2 lastMouseWorldPos;

    void Update()
    {
        // Calculate current camera dimensions
        Vector2 topRightWorldCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        Vector2 bottomLeftWorldCoordinates = Camera.main.ScreenToWorldPoint(Vector2.zero);
        Vector2 cameraWorldDimensions = topRightWorldCoordinates - bottomLeftWorldCoordinates;

        // Get mouse input
        bool isScrolling = Mathf.Abs(Input.mouseScrollDelta.y) > scrollThreshold;
        scrollVelocity += -Input.mouseScrollDelta.y * scrollSensitivity;

        // Scroll
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + scrollVelocity * Time.deltaTime, minZoom, maxZoom);

        if (isScrolling)
        {
            // Update zoom momentum
            lastScrollVelocity = scrollVelocity;
            scrollDampTimer = 0;
            
            // Update mouse position
            lastMouseViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            lastMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            // Update timer
            scrollDampTimer += Time.deltaTime;

            // Dampen scrolling
            scrollVelocity = Mathf.Lerp(lastScrollVelocity, 0, scrollDampTimer / scrollDampTime);
        }

        // Move camera towards target offset from mouse position
        Vector2 scrollTarget = lastMouseWorldPos - cameraWorldDimensions * (lastMouseViewportPos - Vector2.one * 0.5f);
        transform.position = new Vector3(scrollTarget.x, scrollTarget.y, -10);
    }
}
