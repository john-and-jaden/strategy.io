using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 0.05f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    public float sideMoveSpeed = 0.025f;

    void Update()
    {
        // Scrolling
        float scrollDir = -Input.mouseScrollDelta.y;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + scrollDir * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        bool isWithinZoomBounds = Camera.main.orthographicSize < maxZoom && Camera.main.orthographicSize > minZoom;

        if (isWithinZoomBounds) {
            // Moving camera while scrolling
            Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distanceMouseCenter = Vector3.Distance(mousePositionWorld, transform.position);
            // doing distanceMouseCenter + 1 because otherwise log outputs negative values
            float finalSideMoveSpeed = -scrollDir * Mathf.Log(distanceMouseCenter + 1) * Mathf.Log(Camera.main.orthographicSize) * sideMoveSpeed;
            transform.position = Vector3.MoveTowards(transform.position, mousePositionWorld, finalSideMoveSpeed);
        }
    }
}
