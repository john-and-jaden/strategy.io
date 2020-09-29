using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 1f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

    void Update()
    {
        float scrollDir = -Input.mouseScrollDelta.y;
        Camera.main.orthographicSize += scrollDir * zoomSpeed * Time.deltaTime;
    }
}
