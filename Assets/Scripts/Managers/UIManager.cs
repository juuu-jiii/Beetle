using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates UI according to gameplay state.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI livesText;
    [SerializeField]
    private GameObject scoreManager;
    private ScoreManager scoreManagerScript;
    [SerializeField]
    private GameObject player;
    private Cannon playerScript;

    // Panels do not have their own datatype. Create a GameObject variable
    // and drag/drop the panel into the field of this script within the Editor.
    [SerializeField]
    private GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        // Hide the panel on level start.
        Unpause();
        scoreManagerScript = scoreManager.GetComponent<ScoreManager>();
        playerScript = player.GetComponent<Cannon>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO OPTIMISATION: only update when the corresponding event is invoked
        // This means that Events.LifeLost will need to be reincorporated.
        scoreText.text = ScoreManager.Score.ToString();
        livesText.text = "♥ " + Cannon.Lives.ToString();
    }

    /// <summary>
    /// Shows the pause overlay. Called when the game is paused.
    /// </summary>
    public void Pause()
    {
        panel.SetActive(true);
    }

    /// <summary>
    /// Hides the pause overlay. Called when the game is unpaused.
    /// </summary>
    public void Unpause()
    {
        panel.SetActive(false);
    }
}
