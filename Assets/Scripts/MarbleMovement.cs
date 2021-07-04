using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Describes a colour a marble can take.
public enum Colours
{
    Red,
    Yellow,
    Green,
    Blue
}

public class MarbleMovement : MonoBehaviour
{
    public float speed;
    public bool hasRandomDirection;
    public Rigidbody rb;
    private Vector3 previousVelocity;
    public bool isStale;
    public Colours Colour { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 10.0f;
        Colour = (Colours)Random.Range(0, 4);

        switch (Colour)
        {
            case Colours.Red:
                break;
            case Colours.Yellow:
                break;
            case Colours.Green:
                break;
            case Colours.Blue:
                break;
        }

        // Differentiate between projectiles and regular marbles.
        // Projectiles' initial velocities are determined by player aim.
        // Regular marbles have random initial velocities.
        if (hasRandomDirection)
            rb.velocity = new Vector3(
                Random.Range(-1f, 1f), 
                Random.Range(-1f, 1f), 
                Random.Range(-1f, 1f)) * speed;
    }

    // Do everything in FixedUpdate since physics are involved.
    private void FixedUpdate()
    {
        // Marbles all have a constant speed. Fix for top speed @3.5 issue.
        rb.velocity = Vector3.ClampMagnitude(rb.velocity * speed, speed);

        // Get the marble's velocity after each frame for collision handling.
        previousVelocity = rb.velocity;
    }

    // FixedUpdate() is called before OnCollisionEnter(). Leverage this to
    // handle bouncing physics.
    private void OnCollisionEnter(Collision collision)
    {
        MarbleMovement otherMarble = collision.gameObject.GetComponent<MarbleMovement>();

        if (collision.gameObject.tag == "Marble")
        {
            // Colour match and not stale.
            if (Colour == otherMarble.Colour && (!isStale || !otherMarble.isStale))
                // Invoke event.
                // Destroy this and the other marble.
                EventManager.TriggerEvent(
                    Events.MarbleMatch, 
                    this.gameObject, 
                    collision.gameObject);
            else
                // Both marbles are now stale.
                isStale = otherMarble.isStale = true;

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
        }

        Bounce(collision.GetContact(0).normal);
    }

    /// <summary>
    /// "Synthetic" bouncing method that leverages Vector3.Reflect() for a
    /// perfect bounce every time.
    /// </summary>
    /// <param name="collisionNormal">
    /// The surface normal at the point of collision.
    /// </param>
    private void Bounce(Vector3 collisionNormal)
    {
        rb.velocity = Vector3.Reflect(previousVelocity.normalized, collisionNormal) * speed;
    }

    private void Match()
    {

    }
}
