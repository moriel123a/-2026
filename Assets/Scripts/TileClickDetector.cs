using System.Collections;
using UnityEngine;


/// <summary>
/// A class to detect clicks on tiles and gameobjects with clickable.
/// </summary>
public class TileClickDetector : MonoBehaviour
{
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
        InputManager.Instance.OnTap -= HandleTap;
    }

    // Makes sure InputManager created an instance before subscribing
    private IEnumerator SubscribeWhenReady()
    {
        yield return new WaitUntil(() => InputManager.Instance != null);

        InputManager.Instance.OnTap += HandleTap;
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