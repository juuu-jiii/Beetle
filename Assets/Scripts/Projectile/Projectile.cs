using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Marble
{
    private bool isStale;
    private IEnumerator setStaleTimeout;

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
                        EventManager.TriggerEvent(Events.ProjectileMarbleMatch);
                        break;
                    // If the other object is another projectile, code is
                    // executed by whichever marble sets the other inactive
                    // first.
                    case "Projectile":
                        otherMarble.gameObject.SetActive(false);
                        EventManager.TriggerEvent(Events.ProjectileProjectileMatch);
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

            // TODO: Remember to add another Tag in the Editor!
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