using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuStats : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI livesText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "score: " + ScoreManager.Score;

        if (livesText) livesText.text = "lives left: " + Cannon.Lives;
    }
}
