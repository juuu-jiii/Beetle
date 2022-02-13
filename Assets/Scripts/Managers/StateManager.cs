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
    Instructions,
    LevelComplete,
    NextLevel,
    GameOver,
    Win // technically no need for this
}

/// <summary>
/// Handles all zero-argument, void-returning callbacks fired upon state change.
/// </summary>
public delegate void OnStateChangeHandler();

/// <summary>
/// Singleton to streamline game state handling throughout the application.
/// </summary>
public class StateManager : Singleton<StateManager>
{
    private static StateManager instance = null;

    /// <summary>
    /// Total number of menu screens. Used to "skip ahead" in build settings
    /// settings indices, because of the way Scenes are ordered within.
    /// </summary>
    private const int MenuCount = 5;

    /// <summary>
    /// Total number of levels. Used to determine when the player has beaten
    /// the game vs when a new level needs to be loaded.
    /// </summary>
    private const int LevelCount = 1;

    /// <summary>
    /// Event that gets invoked when game state is altered.
    /// </summary>
    // NOTE: The Awake lifecycle hook is vital to this implementation, and for
    // some reason Unity event subscriptions misbehave during this stage. To
    // that end, C# events are used instead.
    public event OnStateChangeHandler OnStateChange;

    /// <summary>
    /// The current game state.
    /// </summary>
    public GameStates GameState { get; private set; }

    //public static StateManager Instance
    //{
    //    get 
    //    { 
    //        if (!instance)
    //        {
    //            // A similar implementation to that of EventManager, except
    //            // the instance must persist throughout the whole application,
    //            // instead of a scene-by-scene basis.

    //            // Search for a StateManager component in the Scene.
    //            instance = FindObjectOfType<StateManager>();

    //            // If FindObjectOfType() returns null, then the component does
    //            // not exist in the scene yet. Create an empty GameObject and
    //            // attach the component to it, since all executable code must
    //            // be attached to an active GameObject within the Hierarchy.
    //            //
    //            // This way, we "lazily" instantiate te StateManager (i.e. only
    //            // instantiate when it is needed. Therefore, the StateManager
    //            // component does not need to exist in the Scene beforehand.
    //            if (!instance)
    //            {
    //                GameObject gObj = new GameObject();
    //                gObj.name = "State Manager";
    //                instance = gObj.AddComponent<StateManager>();

    //                // Because the instance must persist throughout the application,
    //                // it cannot be destroyed in between scenes.
    //                DontDestroyOnLoad(gObj);
    //            }
    //        }

    //        return instance;
    //    }
    //}

    //private void Awake()
    //{
    //    if (!instance)
    //    {
    //        // REMEMBER! Constructors are not used in Unity!
    //        instance = this;

    //        // Because the instance must persist throughout the application,
    //        // it cannot be destroyed in between scenes.
    //        DontDestroyOnLoad(this.gameObject);
    //    }
    //    // Destroy any additional copies of the StateManager in the Scene.
    //    else Destroy(this.gameObject);
    //}

    /// <summary>
    /// Changes the game's state and fires all necessary callbacks.
    /// </summary>
    /// <param name="newState"></param>
    public void SetState(GameStates newState)
    {
        // TODO: load new scenes here.
        GameState = newState;
        OnStateChange.Invoke();
        Debug.Log("Score: " + ScoreManager.Score);

        switch (newState)
        {
            case GameStates.Title:
                SceneManager.LoadSceneAsync("Title");
                break;
            case GameStates.Instructions:
                SceneManager.LoadSceneAsync("Instructions");
                break;
            case GameStates.LevelComplete:
                // Trigger the Win Scene when all levels have been beaten.
                if (GameManager.Level == LevelCount)
                {
                    EventManager.Instance.TriggerEvent(Events.Win);
                    SceneManager.LoadSceneAsync("Win");
                }
                else
                    SceneManager.LoadSceneAsync("Level Complete");
                break;
            case GameStates.NextLevel:
                SceneManager.LoadSceneAsync(GameManager.Level + MenuCount);
                break;
            case GameStates.GameOver:
                SceneManager.LoadSceneAsync("Game Over");
                break;
        }
    }

    //private void OnApplicationQuit()
    //{
    //    // Make instance point to null before the application is quit so the
    //    // memory allocated for it can be garbage collected.
    //    instance = null;
    //}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
