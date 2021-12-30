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

    public void ShowInstructions()
    {
        Debug.Log("showing instructions");
        stateManager.SetState(GameStates.Instructions);
    }

    public void PlayButtonClicked()
    {
        Debug.Log("play clicked");
        stateManager.SetState(GameStates.Level1);
    }

    public void QuitButtonClicked()
    {
        Debug.Log("quit clicked");
        Application.Quit();
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
