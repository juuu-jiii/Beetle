// Taken and adapted from https://github.com/bttfgames/SimpleGameManager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameStates
{
    Title,
    Level1,
    Level2,
    Level3,
    GameOver,
    Win
}

public delegate void OnStateChangeHandler();

public class StateManager : MonoBehaviour
{
    private static StateManager instance = null;
    public event OnStateChangeHandler OnStateChange;
    //public UnityEvent OnStateChange; //{ get; private set; }
    public GameStates GameState { get; private set; }

    public static StateManager Instance
    {
        get 
        { 
            if (!instance)
            {
                instance = FindObjectOfType<StateManager>();

                if (!instance)
                {
                    GameObject go = new GameObject();
                    go.name = "State Manager";
                    instance = go.AddComponent<StateManager>();
                    DontDestroyOnLoad(go);
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        //if (instance) Destroy(this.gameObject);
        //else
        //{
        //    // Set instance = this and instruct Unity to make it persist
        //    // between Scenes.
        //    // REMEMBER! Constructors are not used in Unity!
        //    instance = this;
        //    DontDestroyOnLoad(instance);
        //}
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
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
