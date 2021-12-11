// Taken and adapted from https://github.com/bttfgames/SimpleGameManager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameStates
{
    Title,
    Level1,
    Level2,
    Level3,
    GameOver,
    Win
}

public class StateManager : MonoBehaviour
{
    private static StateManager instance = null;
    private UnityEvent onStateChange;
    public GameStates GameState { get; private set; }

    public static StateManager Instance
    {
        get
        {
            if (!instance)
            {
                // Instantiate the StateManager and instruct Unity to make it
                // persist between Scenes.
                DontDestroyOnLoad(instance);
                instance = new StateManager();
            }

            return instance;
        }
    }

    private StateManager() { }

    /// <summary>
    /// Changes the game's state and fires all necessary callbacks.
    /// </summary>
    /// <param name="newState"></param>
    public void SetState(GameStates newState)
    {
        GameState = newState;
        onStateChange.Invoke();
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
