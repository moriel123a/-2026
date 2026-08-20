using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController2D : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference dragAction;
    [SerializeField] private InputActionReference pointAction;
    [SerializeField] private InputActionReference zoomAction;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 15f;

    private Vector3 dragOrigin;
    private bool isDragging;

    private void OnEnable()
    {
        dragAction.action.Enable();
        pointAction.action.Enable();
        zoomAction.action.Enable();

        dragAction.action.started += OnDragStarted;
        dragAction.action.canceled += OnDragCanceled;
    }

    private void OnDisable()
    {
        dragAction.action.started -= OnDragStarted;
        dragAction.action.canceled -= OnDragCanceled;

        dragAction.action.Disable();
        pointAction.action.Disable();
        zoomAction.action.Disable();
    }

    private void Update()
    {
        HandlePan();
        HandleZoom();
    }

    private void OnDragStarted(InputAction.CallbackContext context)
    {
        dragOrigin = GetPointerWorldPosition();
        isDragging = true;
    }

    private void OnDragCanceled(InputAction.CallbackContext context)
    {
        isDragging = false;
    }

    private void HandlePan()
    {
        if (isDragging)
        {
            Vector3 difference = dragOrigin - GetPointerWorldPosition();
            mainCamera.transform.position += difference;

            GridManager gridInstance = GridManager.Instance;
            Vector2 minPos = gridInstance.GridToWorld(0, 0);
            Vector2 maxPos = gridInstance.GridToWorld(gridInstance.width - 1, gridInstance.height - 1);
            float positionX = Mathf.Clamp(mainCamera.transform.position.x, minPos.x, maxPos.x);
            float positionY = Mathf.Clamp(mainCamera.transform.position.y, minPos.y, maxPos.y);
            
            mainCamera.transform.position = new Vector3(positionX, positionY, mainCamera.transform.position.z);
        }
    }

    private void HandleZoom()
    {
        // Reads value from Scroll (Vector2/Axis) or Pinch gestures
        float zoomInput = zoomAction.action.ReadValue<Vector2>().y;

        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            mainCamera.orthographicSize -= zoomInput * zoomSpeed * Time.deltaTime;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
        }
    }

    private Vector3 GetPointerWorldPosition()
    {
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, mainCamera.nearClipPlane));
        worldPos.z = mainCamera.transform.position.z;
        return worldPos;
    }
}