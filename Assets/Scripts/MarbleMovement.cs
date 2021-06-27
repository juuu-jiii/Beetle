using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory class used for instantiating marbles
/// </summary>
public static class MarbleFactory
{
    /// <summary>
    /// Instantiates a marble, and sets its hasRandomDirection bool. An extension of Object.Instantiate().
    /// </summary>
    /// <returns>
    /// A reference to the instantiated marble, as type Object.
    /// </returns>
    public static Object InstantiateMarble(
        Object original, 
        Vector3 position, 
        Quaternion rotation,
        bool hasRandomDirection)
    {
        GameObject marble = Object.Instantiate(original, position, rotation) as GameObject; // can also perform explicit cast
        MarbleMovement marbleScript = marble.GetComponent<MarbleMovement>();
        marbleScript.hasRandomDirection = hasRandomDirection;
        return marble;
    }
}

public class MarbleMovement : MonoBehaviour
{
    public GameObject sphere;
    public float speed;
    public bool hasRandomDirection;
    public Rigidbody rb;
    private Vector3 previousVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 10.0f;

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
}
