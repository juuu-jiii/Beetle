using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Marbles and projectiles each have different values.
    [SerializeField]
    private int marbleValue;
    [SerializeField]
    private int projectileValue;
    public int Score { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        // Increment scores accordingly based on the event invoked.
        EventManager.StartListening(Events.MarbleMatch, HandleMarbleMatch);
        EventManager.StartListening(Events.ProjectileMatch, HandleProjectileMatch);

        Score = 0;
    }

    public void HandleMarbleMatch()
    {
        Score += marbleValue;
    }

    public void HandleProjectileMatch()
    {
        Score += projectileValue;
    }
}
