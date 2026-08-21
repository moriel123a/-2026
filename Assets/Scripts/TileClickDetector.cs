using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TileClickDetector : MonoBehaviour
{
    void OnEnable()
    {
        InputManager.Instance.OnTap += HandleTap;
    }

    void OnDisable()
    {
        if (InputManager.Instance == null) return;
        InputManager.Instance.OnTap -= HandleTap;
    }

    private void HandleTap(Vector2 screenPos)
    {
        Vector2 clickPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(clickPos);

        if (hit == null) return;

        if (hit.TryGetComponent(out IClickable clickable))
        {
            clickable.HandleClick();
        }
    }
}