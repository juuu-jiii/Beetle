using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles scoring logic.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// Static Score property that persists between Scenes.
    /// </summary>
    public static int Score { get; private set; }

    // Marbles and projectiles each have different values.
    /// <summary>
    /// Points awarded for destroying a marble.
    /// </summary>
    [SerializeField]
    private int marbleValue;

    /// <summary>
    /// Points awarded for destroying a projectile.
    /// </summary>
    [SerializeField]
    private int projectileValue;

    [SerializeField]
    private int targetValue;

    // Start is called before the first frame update
    void Start()
    {
        // Increment scores accordingly based on the event invoked.
        EventManager.StartListening(Events.MarbleMatch, HandleMarbleMatch);
        EventManager.StartListening(Events.ProjectileMatch, HandleProjectileMatch);
        EventManager.StartListening(Events.TargetMatch, HandleTargetMatch);

        //Score = 0;
    }

    /// <summary>
    /// Increments player's score when a marble is destroyed. Invoked as part
    /// of Events.MarbleMatch.
    /// </summary>
    public void HandleMarbleMatch()
    {
        Score += marbleValue;
    }

    /// <summary>
    /// Increments player's score when a projectile is destroyed. Invoked as
    /// part of Events.ProjectileMatch.
    /// </summary>
    public void HandleProjectileMatch()
    {
        Score += projectileValue;
    }

    public void HandleTargetMatch()
    {
        Score += targetValue;
    }

    // TODO LATER: combo handling
    // "combo active" timer
}
