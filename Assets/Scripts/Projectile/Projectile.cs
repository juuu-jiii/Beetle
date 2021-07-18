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

    //// Inherit the same basic behaviours from the parent.
    //private void FixedUpdate()
    //{
    //    base.FixedUpdate();
    //}

    protected override void OnCollisionEnter(Collision collision)
    { 
        // Optimise by only checking for colour matches if this projectile is
        // not stale.
        if (!isStale && 
            (collision.gameObject.tag == "Marble" || collision.gameObject.tag == "Projectile"))
        {
            Marble otherMarble = collision.gameObject.GetComponent<Marble>();

            // TODO: get rid of event params
            // check for item type. if marble DESTROY it too. if projectile just DESTROY self
            // What if, regardless of type, you just destroy both?
            // script order: destroy(that) --> invoke --> destroy(this)
            // invoke event that calls GameManager.HandleMarbleMatch here.
            // Then, in GameManager.HandleMarbleMatch(), Linq through marbles and remove
            //      null refs created by Destroy calls. Each event invocation will 
            //      mean score is incremented once. No need to halve the value anywhere.
            // Just setinactive the other object and destroy it, invoke event, and then destroy
            //      this GameObject. In the case of the other object being a Projectile, it
            //      will be kind of a race condition. Let's see if this works.
            // 
            // Note: if reference to this object is needed as a sender, just create a
            // generic Object param and pass "this" as the param. Optional on whether
            // to use. Should be ok, since C#'s built-in event system does something similar.

            // Deactivate 
            //otherMarble.gameObject.SetActive(false);

            if (Colour == otherMarble.Colour)
            {
                Matched = true;

                switch (collision.gameObject.tag)
                {
                    case "Marble":
                        otherMarble.Matched = true;
                        EventManager.TriggerEvent(Events.ProjectileMarbleMatch);
                        break;
                    case "Projectile":
                        // Code is executed by whichever marble sets the other
                        // inactive first.
                        otherMarble.gameObject.SetActive(false);
                        EventManager.TriggerEvent(Events.ProjectileProjectileMatch);
                        break;
                    default:
                        Debug.LogError("Collider tag was neither Marble nor Projectile!");
                        break;
                }
            }
            else
            {
                StopCoroutine(setStaleTimeout);
                isStale = true;
                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                base.OnCollisionEnter(collision);
            }





            //switch (collision.gameObject.tag)
            //{
            //    case "Marble":
            //        // Colour match:
            //        if (Colour == otherMarble.Colour)
            //        {
            //            EventManager.TriggerEvent(
            //                Events.ProjectileMarbleMatch,
            //                this.gameObject,
            //                new List<object>() {
            //                otherMarble.gameObject
            //                });
            //        }
            //        // Otherwise, this projectile is now stale. Continue bouncing.
            //        else
            //        {
            //            StopCoroutine(setStaleTimeout);
            //            isStale = true;
            //            GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            //            base.OnCollisionEnter(collision);
            //        }
            //        break;
            //    case "Projectile":

            //        break;
            //    default:
            //        Debug.LogError("Collider tag was neither Marble nor Projectile!");
            //        break;
            //}
            
            //Debug.Log("not stale - collision detected");
            // If the other object is a regular marble, destroy both this and the
            // other marble within this script, since regular marbles do not trigger
            // EventManager events themselves when colliding.
            //if (collision.gameObject.tag == "Marble")
            //{
            //    // Colour match:
            //    if (Colour == otherMarble.Colour)
            //    {
            //        // Invoke event: destroy this and the other marble.
            //        EventManager.TriggerEvent(
            //            Events.MarbleMatch,
            //            this.gameObject,
            //            collision.gameObject);
            //        //Debug.Log("Matched");
            //    }
            //    // Otherwise, this projectile is now stale. Continue bouncing.
            //    else
            //    {
            //        //Debug.Log("No match - now stale");
            //        // Remove emission from projectile once stale.
            //        StopCoroutine(setStaleTimeout);
            //        isStale = true;
            //        GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            //        base.OnCollisionEnter(collision);
            //    }
            //}

            // TODO STRETCH: optimise by invoking single-param version of event
            // If I am a projectile, only destroy myself. The other projectile
            // will know how to do the same.
            // This removes the need to do a Contains() check in GameManager.
            // ^^^ Actually still a good idea to keep the Contains() check, on
            // the VERY off chance that two live projectiles collide with a
            // marble at the exact same time. Or maybe not needed? Just remove
            // first and see what happens lah
            // Remember to add another Tag in the Editor!
            // To avoid added scores being doubled, just halve the original.
            // Scores will get added twice but the total will reflect the 
            // correct sum, therefore.
            //else if (collision.gameObject.tag == "Projectile")
            //{

            //}

            
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

/*
    Problem here:
    If both colliding objects are marbles, then the event will
    be called twice
    You could call destroy in each marble's own script.
    To add score, though, you must only add once.
    Since the event gets fired twice, will dividing by 2 work?
    Or the more complex way is to figure out which marble is 
    the "live" one and handle logic from there. But if both
    are live, what's going to happen?

    You can also try passing data about this and the other
    GameObject to an event. See if that works.

    If that does not work then cibai la just maintain
    circular references. Forget it and just move on at least
    that method is guaranteed to work.

    Idea: pass the reference of both this and the other GameObject
    to the event. In the callback where score is increased in the
    GameManager, use these references to do a Contains() lookup
    within the list. If both are found, remove both of them,
    destroy them, and then increment the score from there. The second
    time the callback is fired by the other GameObject, the 
    references will not longer be in the List, and the code after
    the Contains() check will not run, and exceptions caused by
    trying to remove something that isn't there can be avoided.
    This also ensures that score is only added to once.
    Now just pray that remove works in the way you think it does
    so this all can work...
*/