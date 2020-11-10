using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float scrollVelocityDampener = 0.8f;
    private float scrollVelocity = 0f;

    void Update()
    {
        // Keep a record of the mouse position before scrolling
        Vector2 mouseViewportCoordinates = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 mouseWorldCoodinates = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Scroll
        scrollVelocity += -Input.mouseScrollDelta.y;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + scrollVelocity * Time.deltaTime, minZoom, maxZoom);

        // Move camera towards mouse while scrolling
        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 topRightWorldCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        Vector2 bottomLeftWorldCoordinates = Camera.main.ScreenToWorldPoint(Vector2.zero);
        Vector2 cameraDimensionsWorld = topRightWorldCoordinates - bottomLeftWorldCoordinates;
        transform.position = mouseWorldCoodinates - cameraDimensionsWorld * (mouseViewportCoordinates - Vector2.one * 0.5f);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        // Dampen scrolling
        scrollVelocity = Mathf.Lerp(scrollVelocity, 0, scrollVelocityDampener);
    }
}
