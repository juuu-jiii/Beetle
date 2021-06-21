using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject sphere;
    public float speed;
    private float xForce, zForce;
    private Rigidbody rb;
    //private Vector3 position;
    //private Vector3 velocity;
    //private Vector3 acceleration;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 10.0f;
        
        //position = sphere.transform.position;
        //velocity = Vector3.zero;
        //acceleration = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        xForce = zForce = 0;
        //velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow)) zForce += speed;
        //velocity = sphere.transform.forward * speed;
        if (Input.GetKey(KeyCode.DownArrow)) zForce -= speed;
        //velocity = sphere.transform.forward * -speed;
        if (Input.GetKey(KeyCode.RightArrow)) xForce += speed;
        //velocity = sphere.transform.right * speed;
        if (Input.GetKey(KeyCode.LeftArrow)) xForce -= speed;
        //velocity = sphere.transform.right * -speed;

        //rb.AddForce(velocity.x, 0.0f, velocity.y);
        rb.AddForce(xForce, 0.0f, zForce);

        //sphere.transform.position += velocity;

        Debug.Log(sphere.transform.position);
    }
}
