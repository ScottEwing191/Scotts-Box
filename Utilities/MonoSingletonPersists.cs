using Sirenix.OdinInspector;
using UnityEngine;

namespace ScottEwing.Scotts_Box.Utilities{
    using UnityEngine;

public abstract class MonoSingletonPersists<T> : MonoBehaviour where T : MonoSingletonPersists<T>
{
    private static T _instance;
    private static bool _isApplicationQuitting;
    private static bool _isInitialized;

    public static T Instance
    {
        get
        {
            // Don't create instances during shutdown
            if (_isApplicationQuitting)
            {
                return null;
            }

            // Return existing instance if available
            if (_instance != null)
            {
                // Initialize if not already done
                if (!_isInitialized)
                {
                    _isInitialized = true;
                    _instance.Init();
                }
                return _instance;
            }

            // Look for existing instance in scene
            _instance = FindObjectOfType<T>();

            // Create new instance if none found
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject(typeof(T).Name);
                DontDestroyOnLoad(singletonObject);
                _instance = singletonObject.AddComponent<T>();
                // Note: _instance is set in Awake() which runs immediately during AddComponent
            }

            // Initialize if not already done
            if (!_isInitialized)
            {
                _isInitialized = true;
                _instance.Init();
            }

            return _instance;
        }
    }

    [Button]
    private void DontDestroy() {
        DontDestroyOnLoad(gameObject);
    }
    
    protected virtual void Awake()
    {
        // If this is the first instance, keep it
        if (_instance == null)
        {
            _instance = this as T;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            
            
            // Initialize if not already done (for scene-placed instances)
            if (!_isInitialized)
            {
                _isInitialized = true;
                Init();
            }
            return;
        }

        // If another instance already exists, destroy this duplicate
        if (_instance != this)
        {
            Debug.LogWarning($"Duplicate {typeof(T).Name} found. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called once when the singleton is first accessed.
    /// Override this instead of Awake() for singleton initialization logic.
    /// </summary>
    protected virtual void Init()
    {
        // Override in derived classes for initialization
    }

    protected virtual void OnDestroy() { ;
        // Clear reference if this instance is being destroyed
        if (_instance == this)
        {
            _instance = null;
            _isInitialized = false;
        }
    }

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    // Handle Unity's "Enter Play Mode Options" - clears static references when exiting play mode
    // Was never getting called
    /*[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _instance = null;
        _isApplicationQuitting = false;
        _isInitialized = false;
    }*/
}
}