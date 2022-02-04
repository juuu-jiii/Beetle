using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    private StateManager stateManager;

    private void Awake()
    {
        
    }

    // Called whenever state changes.
    public void HandleOnStateChange()
    {
        Debug.Log("OnStateChange invoked");
    }

    public void BackToTitle()
    {
        Debug.Log("back to title");
        EventManager.TriggerEvent(Events.Restart);
        stateManager.SetState(GameStates.Title);
    }

    public void ShowInstructions()
    {
        Debug.Log("showing instructions");
        stateManager.SetState(GameStates.Instructions);
    }

    public void LoadNextLevel()
    {
        Debug.Log("loading level " + GameManager.Level);
        stateManager.SetState(GameStates.NextLevel);
    }

    public void QuitButtonClicked()
    {
        Debug.Log("quit clicked");
        Application.Quit();
    }

    public void Retry()
    {
        Debug.Log("retry clicked");
        EventManager.TriggerEvent(Events.Restart);
        LoadNextLevel();
    }

    // Start is called before the first frame update
    void Start()
    {
        // This must not be done in Awake; the order in which the method gets
        // called across scripts is uncertain, meaning it is possible to obtain
        // a NullReferenceException.
        stateManager = StateManager.Instance;
        stateManager.OnStateChange += HandleOnStateChange;
        //stateManager.OnStateChange.AddListener(HandleOnStateChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
