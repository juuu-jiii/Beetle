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
    // TODO DELETE: deprecated
    //public bool hasRandomDirection;
    public Rigidbody rb;
    protected Vector3 previousVelocity;
    public Colours Colour { get; protected set; }
    // TODO LATER: add staleness timer

    // Start is called before the first frame update
    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 10.0f;
        //Colour = (Colours)Random.Range(0, 4);

        //switch (Colour)
        //{
        //    case Colours.Red:
        //        break;
        //    case Colours.Yellow:
        //        break;
        //    case Colours.Green:
        //        break;
        //    case Colours.Blue:
        //        break;
        //}

        // TODO DELETE: deprecated
        // Differentiate between projectiles and regular marbles.
        // Projectiles' initial velocities are determined by player aim.
        // Regular marbles have random initial velocities.
        //if (hasRandomDirection)
        //    rb.velocity = new Vector3(
        //        Random.Range(-1f, 1f), 
        //        Random.Range(-1f, 1f), 
        //        Random.Range(-1f, 1f)) * speed;

        // TODO: initialise velocity
        // since marbles do not have random directions of motion any
        // longer, it might be necessary to set an initial speed in the
        // direction they are facing.
    }

    // Do everything in FixedUpdate since physics are involved.
    protected void FixedUpdate()
    {
        // Marbles all have a constant speed. Fix for top speed @3.5 issue.
        rb.velocity = Vector3.ClampMagnitude(rb.velocity * speed, speed);

        // Get the marble's velocity after each frame for collision handling.
        previousVelocity = rb.velocity;
    }

    // FixedUpdate() is called before OnCollisionEnter(). Leverage this to
    // handle bouncing physics.
    protected virtual void OnCollisionEnter(Collision collision)
    {
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
