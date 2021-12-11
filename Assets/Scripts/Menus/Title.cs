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

    public void PlayButtonClicked()
    {
        stateManager.SetState(GameStates.Level1);
        Debug.Log("play clicked");
    }

    public void QuitButtonClicked()
    {
        Debug.Log("quit clicked");
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        stateManager = StateManager.Instance;
        stateManager.OnStateChange += HandleOnStateChange;
        //stateManager.OnStateChange.AddListener(HandleOnStateChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
