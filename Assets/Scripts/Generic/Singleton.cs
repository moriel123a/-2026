using UnityEngine;

/// <summary>
/// A generic singleton class
/// Inherit from it to make a class into a singleton.
/// </summary>
/// <typeparam name="T">The class you want to make a singleton from</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this as T)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
    }
}
