using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        scoreManagerScript = scoreManager.GetComponent<ScoreManager>();
        playerScript = player.GetComponent<Cannon>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO OPTIMISATION: only update when the corresponding event is invoked
        // This means that Events.LifeLost will need to be reincorporated.
        scoreText.text = scoreManagerScript.Score.ToString();
        livesText.text = "♥ " + playerScript.Lives.ToString();
    }
}
