using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Describes the colours marbles could have.
public enum Colours
{
    Red,
    Jaune,
    Green,
    Blue
}

// Note: materials list is currently placed in GameManager, so each marble does
// not need to maintain its own list. Unless there is a need to do so (maybe in
// the case of special marbles that change colour at regular intervals), this
// should remain untouched.

/// <summary>
/// Describes the properties/behaviours of a marble object.
/// </summary>
public class Marble : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    protected Vector3 previousVelocity;
    public bool Matched { get; set; }
    public Colours Colour { get; set; }
    private Material material;

    /// <summary>
    /// get property that ensures the RigidBody reference to this marble, rb,
    /// exists before returning it. Ensures that references are resolved during
    /// runtime when marbles get dynamically instantiated.
    /// </summary>
    // Note that calling Instantiate() does not call that GameObject's Start()
    // because Start() is not a constructor! Start() gets called for all newly-
    // created GameObjects at the same time, much like Update(). Since the 
    // velocities of marbles are assigned immediately after being spawned, this
    // implementation prevents UnassignedReferenceExceptions. This way, there
    // is no need to initialise rb within Start().
    public Rigidbody Rb
    {
        get
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            return rb;
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        speed = 10.0f;
    }

    // Handle movement in FixedUpdate, since physics are involved.
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
    /// "Synthetic" bouncing method that leverages Vector3.Reflect().
    /// </summary>
    /// <param name="collisionNormal">
    /// The surface normal at the point of collision.
    /// </param>
    private void Bounce(Vector3 collisionNormal)
    {
        rb.velocity = Vector3.Reflect(previousVelocity.normalized, collisionNormal) * speed;
    }
}
