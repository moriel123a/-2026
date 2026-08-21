using System.Collections;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 1f; // 1 means the same location will always stay under you pointer
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 15f;

    private Coroutine subscribeRoutine;

    private void OnEnable()
    {
        subscribeRoutine = StartCoroutine(SubscribeWhenReady());
    }

    private void OnDisable()
    {
        if (subscribeRoutine != null)
        {
            StopCoroutine(subscribeRoutine);
            subscribeRoutine = null;
        }

        if (InputManager.Instance == null) return;
        InputManager.Instance.OnDrag -= HandlePan;
        InputManager.Instance.OnZoom -= HandleZoom;
    }

    // Makes sure InputManager created an instance before subscribing
    private IEnumerator SubscribeWhenReady()
    {
        yield return new WaitUntil(() => InputManager.Instance != null);

        InputManager.Instance.OnDrag += HandlePan;
        InputManager.Instance.OnZoom += HandleZoom;
    }

    private void HandlePan(Vector2 currentPosition, Vector2 delta)
    {
        // Convert the screen-space delta into an actual world-space delta by comparing where
        // the previous and current pointer positions land in the world. This keeps panning
        // 1:1 (the point under the cursor stays under the cursor) at any zoom level, instead
        // of applying raw pixel counts directly to world-unit position.

        float z = mainCamera.nearClipPlane;
        Vector3 previousWorld = mainCamera.ScreenToWorldPoint(new Vector3(currentPosition.x - delta.x, currentPosition.y - delta.y, z));
        Vector3 currentWorld = mainCamera.ScreenToWorldPoint(new Vector3(currentPosition.x, currentPosition.y, z));

        Vector3 worldDelta = currentWorld - previousWorld;
        mainCamera.transform.position -= worldDelta * panSpeed;

        GridManager gridInstance = GridManager.Instance;
        Vector2 minPos = gridInstance.GridToWorld(0, 0);
        Vector2 maxPos = gridInstance.GridToWorld(gridInstance.width - 1, gridInstance.height - 1);
        float positionX = Mathf.Clamp(mainCamera.transform.position.x, minPos.x, maxPos.x);
        float positionY = Mathf.Clamp(mainCamera.transform.position.y, minPos.y, maxPos.y);

        mainCamera.transform.position = new Vector3(positionX, positionY, mainCamera.transform.position.z);
    }

    private void HandleZoom(float zoomInput)
    {
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            mainCamera.orthographicSize -= zoomInput * zoomSpeed * Time.deltaTime;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
        }
    }
}