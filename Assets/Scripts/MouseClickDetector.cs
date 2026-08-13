using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClickDetector : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryClickAtMouse();
        }
    }

    private void TryClickAtMouse()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 clickPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(clickPos);

        if (hit == null) return;

        if (hit.TryGetComponent(out IClickable clickable))
        {
            clickable.HandleClick();
        }
    }
}