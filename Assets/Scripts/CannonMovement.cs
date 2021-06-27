using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonMovement : MonoBehaviour
{
    public GameObject cannon;
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
        // Reset each frame so key presses can be tracked correctly.
        velocity = Vector3.zero;

        // Strafe left and right.
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) velocity = Vector3.left * speed;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) velocity = Vector3.right * speed;

        // AddForce() cannot be used - movement must be immediate.
        cannon.transform.position += velocity;
    }
}
