using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonMovement : MonoBehaviour
{
    //public GameObject cannon;
    public float speed;
    private Rigidbody rb;
    private Vector3 velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Do everything in FixedUpdate since physics are involved.
    void FixedUpdate()
    {
        // Reset each frame so any movement is updated correctly.
        velocity = Vector3.zero;
    }

    public void StrafeLeft()
    {
        velocity = Vector3.left * speed;

        // AddForce() cannot be used - movement must be immediate.
        transform.position += velocity;
    }

    public void StrafeRight()
    {
        velocity = Vector3.right * speed;
        // AddForce() cannot be used - movement must be immediate.
        transform.position += velocity;
    }
}
