using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minZoom = 5f;
    public float maxZoom = 15f;
    [Tooltip("The scroll damp time in seconds")]
    public float scrollDampTime = 0.8f;
    public float scrollSensitivity = 1f;
    public float scrollThreshold = 0f;

    private float scrollVelocity = 0f;
    private float timer;
    private float lastScrollVelocity = 0f;
    private float interpolant = 0f;
    private Vector2 scrollTarget;
    private bool isScrolling;

    void Update()
    {
        // Keep a record of the mouse position before scrolling
        Vector2 mouseViewportCoordinates = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 mouseWorldCoodinates = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Scroll
        isScrolling = Mathf.Abs(Input.mouseScrollDelta.y) > scrollThreshold;
        scrollVelocity += -Input.mouseScrollDelta.y * scrollSensitivity;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + scrollVelocity * Time.deltaTime, minZoom, maxZoom);

        if (isScrolling)
        {
            // Update values
            lastScrollVelocity = scrollVelocity;
            timer = 0;
            float cameraHeight = Camera.main.orthographicSize * 2;
            Vector2 topRightWorldCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
            Vector2 bottomLeftWorldCoordinates = Camera.main.ScreenToWorldPoint(Vector2.zero);
            Vector2 cameraDimensionsWorld = topRightWorldCoordinates - bottomLeftWorldCoordinates;
            scrollTarget = mouseWorldCoodinates - cameraDimensionsWorld * (mouseViewportCoordinates - Vector2.one * 0.5f);
        }
        else
        {
            // Update timer
            timer += Time.deltaTime;

            // Dampen scrolling
            scrollVelocity = Mathf.Lerp(lastScrollVelocity, 0, timer / scrollDampTime);
        }

        // Move camera towards scrollTarget
        transform.position = new Vector3(scrollTarget.x, scrollTarget.y, -10);
    }
}
