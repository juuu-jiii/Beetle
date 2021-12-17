// Taken from https://www.unitygeek.com/unity_c_singleton/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic singleton implementation for data persistence throughout program
/// runtime.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        // A similar implementation to that of EventManager, except
        // the instance must persist throughout the whole application,
        // instead of a scene-by-scene basis.

        get
        {
            if (!instance)
            {
                // Search for a component of type T in the Scene.
                instance = FindObjectOfType<T>();

                // If FindObjectOfType() returns null, then the component does
                // not exist in the scene yet. Create an empty GameObject and
                // attach the component to it, since all executable code must
                // be attached to an active GameObject within the Hierarchy.
                //
                // This way, we "lazily" instantiate the singleton (i.e. only
                // create it when it is needed. Therefore, the component, and
                // therefore the GameObject it needs to be attached to, do not
                // need to exist in the Scene beforehand.
                if (!instance)
                {
                    GameObject gObj = new GameObject();
                    gObj.name = typeof(T).Name;
                    instance = gObj.AddComponent<T>();
                }
            }

            return instance;
        }
    }
    
    public virtual void Awake()
    {
        if (!instance)
        {
            // REMEMBER! Constructors are not used in Unity!
            instance = this as T;

            // Because the instance must persist throughout the application,
            // it cannot be destroyed in between scenes.
            DontDestroyOnLoad(this.gameObject);
        }
        // Destroy any additional copies of the StateManager in the Scene.
        else Destroy(this.gameObject);
    }

    protected void OnApplicationQuit()
    {
        // Make instance point to null before the application is quit so the
        // memory allocated for it can be garbage collected.
        instance = null;
    }
}
