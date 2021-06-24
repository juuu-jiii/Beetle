using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject sphere;
    public float speed;
    private float xForce, zForce;
    private Rigidbody rb;
    private Vector3 previousVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 10.0f;
    }

    // Do everything in FixedUpdate since physics are involved.
    private void FixedUpdate()
    {
        xForce = zForce = 0;

        if (rb.velocity.sqrMagnitude <= speed * speed)
        {
            if (Input.GetKey(KeyCode.RightArrow)) xForce += speed;
            //velocity = sphere.transform.right * speed;
            if (Input.GetKey(KeyCode.LeftArrow)) xForce -= speed;
            //velocity = sphere.transform.right * -speed;
            if (Input.GetKey(KeyCode.UpArrow)) zForce += speed;
            //velocity = sphere.transform.forward * speed;
            if (Input.GetKey(KeyCode.DownArrow)) zForce -= speed;
            //velocity = sphere.transform.forward * -speed;
            
            rb.AddForce(xForce, 0.0f, zForce);

            if (Input.GetKey(KeyCode.Space)) rb.AddForce(Vector3.ClampMagnitude(rb.velocity * 100, speed));
        }

        Debug.Log(string.Format("xForce = {0}, zForce = {1}", xForce, zForce));

        // Get the marble's velocity after each frame for collision handling.
        previousVelocity = rb.velocity;

        // TODO: why does the speed decrease when above 3.5 and you let go?
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
