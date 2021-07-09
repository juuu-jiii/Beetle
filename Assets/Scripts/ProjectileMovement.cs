using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MarbleMovement
{
    private bool isStale;

    // Since this class inherits from MarbleMovement, Start and FixedUpdate are
    // also inherited.

    //// Start is called before the first frame update
    //void Start()
    //{
    //    base.Start();
    //}

    //// Inherit the same basic behaviours from the parent.
    //private void FixedUpdate()
    //{
    //    base.FixedUpdate();
    //}

    protected override void OnCollisionEnter(Collision collision)
    { 
        MarbleMovement otherMarble = collision.gameObject.GetComponent<MarbleMovement>();

        // Optimise by only checking for colour matches if this projectile is
        // not stale.
        if (!isStale)
        {
            // If the other object is a regular marble, destroy both this and the
            // other marble within this script, since regular marbles do not trigger
            // EventManager events themselves when colliding.
            if (collision.gameObject.tag == "Marble")
            {
                // Colour match:
                if (Colour == otherMarble.Colour)
                    // Invoke event: destroy this and the other marble.
                    EventManager.TriggerEvent(
                        Events.MarbleMatch,
                        this.gameObject,
                        collision.gameObject);
                // Otherwise, this projectile is now stale. Continue bouncing.
                else
                {
                    isStale = true;
                    base.OnCollisionEnter(collision);
                }
            }
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
        else
        {
            base.OnCollisionEnter(collision);
        }
        
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