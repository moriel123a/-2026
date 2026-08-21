using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Turns raw press/point/zoom Input Actions into higher-level tap/drag/zoom events that
// any consumer (camera, tile clicks, etc.) can subscribe to without caring what device
// produced them. Swapping the mouse bindings for touch bindings later shouldn't require
// changing this class at all - only the Input Actions asset's bindings.
public class InputManager : Singleton<InputManager>
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference pressAction; // Button - held down
    [SerializeField] private InputActionReference pointAction; // Vector2 - pointer/touch position
    [SerializeField] private InputActionReference zoomAction;  // Vector2 (mouse scroll) - .y is used as the zoom amount

    [Header("Tap vs Drag")]
    [Tooltip("If the pointer moves further than this (in screen pixels) while held, it counts as a drag instead of a tap.")]
    [SerializeField] private float dragThreshold = 10f;

    public event Action<Vector2> OnTap; // fired on release, if it wasn't a drag
    public event Action<Vector2> OnDragStart; // fired once, the moment movement crosses the threshold
    public event Action<Vector2, Vector2> OnDrag; // (currentPosition, deltaSinceLastFrame), fired while dragging
    public event Action OnDragEnd; // fired on release, only if a drag was in progress
    public event Action<float> OnZoom; // positive = zoom in, negative = zoom out

    private bool isPressed;
    private bool isDragging;
    private Vector2 pressStartPosition;
    private Vector2 lastPosition;

    void OnEnable()
    {
        pressAction.action.Enable();
        pointAction.action.Enable();
        zoomAction.action.Enable();

        pressAction.action.started += OnPressStarted;
        pressAction.action.canceled += OnPressCanceled;
        zoomAction.action.performed += OnZoomPerformed;
    }

    void OnDisable()
    {
        pressAction.action.started -= OnPressStarted;
        pressAction.action.canceled -= OnPressCanceled;
        zoomAction.action.performed -= OnZoomPerformed;
    }

    void Update()
    {
        if (!isPressed) return;

        Vector2 currentPosition = pointAction.action.ReadValue<Vector2>();

        if (!isDragging)
        {
            if (Vector2.Distance(currentPosition, pressStartPosition) >= dragThreshold)
            {
                isDragging = true;
                lastPosition = currentPosition;
                OnDragStart?.Invoke(pressStartPosition);
            }
            return;
        }

        Vector2 delta = currentPosition - lastPosition;
        if (delta != Vector2.zero)
        {
            OnDrag?.Invoke(currentPosition, delta);
            lastPosition = currentPosition;
        }
    }

    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        isPressed = true;
        isDragging = false;
        pressStartPosition = pointAction.action.ReadValue<Vector2>();
        lastPosition = pressStartPosition;
    }

    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        isPressed = false;

        if (isDragging)
        {
            isDragging = false;
            OnDragEnd?.Invoke();
        }
        else
        {
            OnTap?.Invoke(pointAction.action.ReadValue<Vector2>());
        }
    }

    private void OnZoomPerformed(InputAction.CallbackContext ctx)
    {
        float amount = ctx.ReadValue<Vector2>().y;
        if (amount != 0f)
        {
            OnZoom?.Invoke(amount);
        }
    }
}