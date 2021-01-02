using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Scrolling Settings")]
    [SerializeField] private float zoomDampTime = 0.5f;
    [SerializeField] private float zoomSensitivity = 1f;
    [SerializeField] private float minCameraSize = 5f;
    [SerializeField] private bool invertScrolling = false;

    [Header("Panning Settings")]
    [SerializeField] private float panDampTime = 0.5f;
    [SerializeField] private float maxPanSpeed = 20f;
    [SerializeField] private float panAcceleration = 0.1f;
    [Tooltip("The amount of space in world units beyond the map extents that the camera can view.")]
    [SerializeField] private int worldPanMargin = 10;
    [Tooltip("The proportion of the screen which detects mouse input to pan the camera.")]
    [SerializeField] private float panScreenEdgeProportion = 0.1f;

    Dampable xEdgeSpeedManager;
    Dampable yEdgeSpeedManager;
    Dampable zoomSpeedManager;

    private float worldWidth;
    private float worldHeight;
    private float maxCameraSize;

    void Start()
    {
        // Set class fields
        worldWidth = GameManager.GridSystem.GetDimensions().x;
        worldHeight = GameManager.GridSystem.GetDimensions().y;
        maxCameraSize = worldHeight / 2 + worldPanMargin;
        xEdgeSpeedManager = new Dampable(panDampTime, -maxPanSpeed, maxPanSpeed);
        yEdgeSpeedManager = new Dampable(panDampTime, -maxPanSpeed, maxPanSpeed);
        zoomSpeedManager = new Dampable(zoomDampTime);
    }

    void Update()
    {
        // Get scroll input and zoom the camera
        float scrollInput = GetScrollInput();
        Zoom(scrollInput);

        // Get movement inputs and pan the camera
        Vector2 keyboardPanInput = GetKeyboardPanInput();
        Vector2 mousePanInput = GetMousePanInput();
        Pan(keyboardPanInput + mousePanInput);

        // Clamp camera position to world bounds
        ClampCameraPosition();
    }

    private float GetScrollInput()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel") * (invertScrolling ? 1 : -1);
        zoomSpeedManager.UpdateSpeed(scrollInput * zoomSensitivity);
        return zoomSpeedManager.Speed;
    }

    private void Zoom(float sizeDelta)
    {
        // Clamp size delta
        float currentCameraSize = Camera.main.orthographicSize;
        float targetCameraSize = Mathf.Clamp(currentCameraSize + sizeDelta, minCameraSize, maxCameraSize);
        float clampedSizeDelta = targetCameraSize - currentCameraSize;

        // If zooming in, move camera according to desired zoom amount and mouse position
        if (clampedSizeDelta < 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float changeRatio = clampedSizeDelta / currentCameraSize;
            Vector3 cameraDelta = (mousePosition - transform.position) * changeRatio * -1;
            transform.position += cameraDelta;
        }

        // Zoom camera
        Camera.main.orthographicSize += clampedSizeDelta;
    }

    private Vector2 GetKeyboardPanInput()
    {
        Vector2 keyboardPanInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        return keyboardPanInput * maxPanSpeed;
    }

    private Vector2 GetMousePanInput()
    {
        Vector2 mouseViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        int xDir = mouseViewportPos.x < panScreenEdgeProportion ? -1 : (mouseViewportPos.x > 1 - panScreenEdgeProportion ? 1 : 0);
        int yDir = mouseViewportPos.y < panScreenEdgeProportion ? -1 : (mouseViewportPos.y > 1 - panScreenEdgeProportion ? 1 : 0);

        xEdgeSpeedManager.UpdateSpeed(panAcceleration * xDir * Time.deltaTime);
        yEdgeSpeedManager.UpdateSpeed(panAcceleration * yDir * Time.deltaTime);

        return new Vector2(xEdgeSpeedManager.Speed, yEdgeSpeedManager.Speed);
    }

    private void Pan(Vector2 panDelta)
    {
        float clampedX = Mathf.Clamp(panDelta.x, -maxPanSpeed, maxPanSpeed);
        float clampedY = Mathf.Clamp(panDelta.y, -maxPanSpeed, maxPanSpeed);
        transform.position += new Vector3(clampedX, clampedY) * Time.deltaTime;
    }

    private void ClampCameraPosition()
    {
        // Calculate y bounds
        float cameraSize = Camera.main.orthographicSize;
        float boundY = worldHeight / 2 - cameraSize + worldPanMargin;

        // Calculate zoom level
        float maxZoomLevel = maxCameraSize - minCameraSize;
        float zoomLevel = maxCameraSize - cameraSize;

        // Calculate x bounds based on zoom level
        float minCameraSizeX = minCameraSize * Camera.main.aspect;
        float maxBoundX = worldWidth / 2 - minCameraSizeX + worldPanMargin;
        float boundX = (zoomLevel / maxZoomLevel) * maxBoundX;

        // Calculate clamped position values
        float clampedX = Mathf.Clamp(transform.position.x, -boundX, boundX);
        float clampedY = Mathf.Clamp(transform.position.y, -boundY, boundY);

        // Clamp the camera position
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
