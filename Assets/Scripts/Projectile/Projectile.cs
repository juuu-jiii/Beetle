using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes the properties/behaviours of a projectile object.
/// </summary>
public class Projectile : Marble
{
    /// <summary>
    /// Tracks whether this projectile can still destroy other
    /// marbles/projectiles.
    /// </summary>
    private bool isStale;

    /// <summary>
    /// Reference to SetStaleTimeout() coroutine method.
    /// </summary>
    private IEnumerator setStaleTimeout;

    /// <summary>
    /// In the event of a live collision with another projectile, tracks 
    /// whether this projectile is responsible for running the corresponding 
    /// logic.
    /// </summary>
    public bool IsExecutor { get; set; }

    // Since this class inherits from MarbleMovement, Start and FixedUpdate are
    // also inherited.

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Store a reference to the coroutine so that it can be stopped later.
        // This is very helpful for stopping coroutines accepting params, and
        // is also a much more efficient way of stopping coroutines, compared
        // to using the string overload.
        setStaleTimeout = SetStaleTimeout(3f);
        StartCoroutine(setStaleTimeout);

        IsExecutor = true;
    }

    protected override void OnCollisionEnter(Collision collision)
    { 
        // Optimise by only checking for colour matches if this projectile is
        // not stale.
        if (!isStale && 
            (collision.gameObject.tag == "Marble" || collision.gameObject.tag == "Projectile"))
        {
            Marble otherMarble = collision.gameObject.GetComponent<Marble>();

            // Note: if sender/eventArgs are required moving forward, just
            // create a generic Object param and pass "this" as sender.
            // eventArgs can be a List<Object>. Params can be optional, and
            // could default to a value of null.

            // Colour match:
            if (Colour == otherMarble.Colour)
            {
                Matched = true;

                switch (collision.gameObject.tag)
                {
                    // Different events are invoked depending on whether the
                    // projectile collides with a marble or another projectile,
                    // to allow for different scores to be applied.

                    // If the other object is a regular marble, destroy both
                    // this and the other marble within this script, since  
                    // regular marbles do not trigger EventManager events
                    // themselves when colliding.
                    case "Marble":
                        otherMarble.Matched = true;
                        EventManager.TriggerEvent(Events.MarbleMatch);
                        break;
                    // If the other object is another projectile, code is
                    // executed by whichever marble sets the other's IsExecutor
                    // to false first.
                    //
                    // Note that setting isActive to false only stops life-cycle
                    // functions (Update, OnCollisionEnter, etc.) from being
                    // called. Regular functions - like this one - will still
                    // run, meaning it is not a good way to distinguish between
                    // the two colliding Projectiles.
                    case "Projectile":
                        if (IsExecutor)
                        {
                            // This prevents the other Projectile from executing
                            // the same code block within the same Update frame.
                            // Recall that Destroy is only called after each
                            // Update loop. Why is this important, then?
                            otherMarble.gameObject.GetComponent<Projectile>().IsExecutor = false;

                            // Ensure the other Projectile is also destroyed
                            // when GameManager.ClearMatches() is called as part
                            // of the Events.ProjectileMatch invocation.
                            otherMarble.gameObject.GetComponent<Projectile>().Matched = true;
                            EventManager.TriggerEvent(Events.ProjectileMatch);
                        }
                        break;
                    default:
                        Debug.LogError("Collider tag was neither Marble nor Projectile!");
                        break;
                }
            }
            // Otherwise, this projectile is now stale.
            else
            {
                // Remove emission from projectile once stale, and continue
                // bouncing.
                StopCoroutine(setStaleTimeout);
                isStale = true;
                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                base.OnCollisionEnter(collision);
            }
        }
        else base.OnCollisionEnter(collision);
    }

    /// <summary>
    /// Marks this projectile as stale after a specified duration.
    /// </summary>
    /// <param name="duration">
    /// Length of time post-spawning before this projectile is marked stale.
    /// </param>
    /// <returns>
    /// Object of type IEnumerator - required for coroutine to work.
    /// </returns>
    private IEnumerator SetStaleTimeout(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStale = true;
        GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
    }
}