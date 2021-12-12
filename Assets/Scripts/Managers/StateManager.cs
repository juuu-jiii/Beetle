// Taken and adapted from https://github.com/bttfgames/SimpleGameManager and
// https://www.unitygeek.com/unity_c_singleton/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// All possible states the game can be in.
/// </summary>
public enum GameStates
{
    Title,
    Level1,
    Level2,
    Level3,
    GameOver,
    Win
}

/// <summary>
/// Handles all zero-argument, void-returning callbacks fired upon state change.
/// </summary>
public delegate void OnStateChangeHandler();

/// <summary>
/// Singleton to streamline game state handling throughout the application.
/// </summary>
public class StateManager : MonoBehaviour
{
    private static StateManager instance = null;

    /// <summary>
    /// Event that gets invoked when game state is altered.
    /// </summary>
    // NOTE: The Awake lifecycle hook is vital to this implementation, and for
    // some reason Unity event subscriptions misbehave during this stage. For
    // that reason, C# events are used instead.
    public event OnStateChangeHandler OnStateChange;

    /// <summary>
    /// The current game state.
    /// </summary>
    public GameStates GameState { get; private set; }

    public static StateManager Instance
    {
        get 
        { 
            if (!instance)
            {
                // A similar implementation to that of EventManager, except
                // the instance must persist throughout the whole application,
                // instead of a scene-by-scene basis.

                // Search for a StateManager component in the Scene.
                instance = FindObjectOfType<StateManager>();

                // If FindObjectOfType() returns null, then the component does
                // not exist in the scene yet. Create an empty GameObject and
                // attach the component to it, since all executable code must
                // be attached to an active GameObject within the Hierarchy.
                //
                // This way, we "lazily" instantiate te StateManager (i.e. only
                // instantiate when it is needed. Therefore, the StateManager
                // component does not need to exist in the Scene beforehand.
                if (!instance)
                {
                    GameObject gObj = new GameObject();
                    gObj.name = "State Manager";
                    instance = gObj.AddComponent<StateManager>();

                    // Because the instance must persist throughout the application,
                    // it cannot be destroyed in between scenes.
                    DontDestroyOnLoad(gObj);
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (!instance)
        {
            // REMEMBER! Constructors are not used in Unity!
            instance = this;

            // Because the instance must persist throughout the application,
            // it cannot be destroyed in between scenes.
            DontDestroyOnLoad(this.gameObject);
        }
        // Destroy any additional copies of the StateManager in the Scene.
        else Destroy(this.gameObject);
    }

    /// <summary>
    /// Changes the game's state and fires all necessary callbacks.
    /// </summary>
    /// <param name="newState"></param>
    public void SetState(GameStates newState)
    {
        // TODO: load new scenes here.
        GameState = newState;
        OnStateChange.Invoke();

        switch (newState)
        {
            case GameStates.Title:
                SceneManager.LoadScene("Title");
                break;
            case GameStates.Level1:
                SceneManager.LoadScene("POC Level");
                break;
        }
    }

    private void OnApplicationQuit()
    {
        // Make instance point to null before the application is quit so the
        // memory allocated for it can be garbage collected.
        instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
