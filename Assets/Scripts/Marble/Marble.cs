using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes the properties/behaviours of a marble object.
/// </summary>
public class Marble : MonoBehaviour, IDestructible
{
    [SerializeField]
    protected float topSpeed;
    protected float speed;
    private Rigidbody rb;
    public Colours Colour { get; private set; }
    public bool Matched { get; set; }
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    public float TopSpeed
    {
        get { return topSpeed; }
    }
    /// <summary>
    /// The marble's velocity during the previous frame.
    /// </summary>
    protected Vector3 previousVelocity;


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

    // This assignment is time-sensitive, since the speed property is accessed
    // almost immediately after instantiation in the MarbleSpawner class.
    //
    // This cannot be fixed by altering script execution order, since multiple
    // instantiations will be taking place over time. Thus, changing script
    // execution order will be useful if:
    // 1) only a single instantiation is needed during program runtime, AND
    // 2) the issue is getting the same lifecycle hooks in different scripts
    //      to execute in a specific order.
    private void Awake()
    {
        speed = topSpeed;
    }

    // Handle movement in FixedUpdate, since physics are involved.
    private void FixedUpdate()
    {
        MaintainSpeed(speed);
    }

    // FixedUpdate() is called before OnCollisionEnter(). Leverage this to
    // handle bouncing physics.
    protected virtual void OnCollisionEnter(Collision collision)
    {
        // Use Vector3.Reflect() if colliding with anything other than another
        // marble - this fixes inaccurate collision resolution between two
        // marbles when the angle between them is small.
        if (!collision.gameObject.CompareTag("Projectile")
            && !collision.gameObject.CompareTag("Marble"))
            Bounce(collision.GetContact(0).normal);
    }

    /// <summary>
    /// A workaround fix for top speed @3.5 issue.
    /// </summary>
    /// <param name="maintainSpeed">
    /// The target speed value to maintain.
    /// </param>
    protected void MaintainSpeed(float maintainSpeed)
    {
        // Marbles all have a constant speed.
        rb.velocity = Vector3.ClampMagnitude(rb.velocity * maintainSpeed, maintainSpeed);

        // Get the marble's velocity after each frame for collision handling.
        previousVelocity = rb.velocity;
    }

    /// <summary>
    /// "Synthetic" bouncing method that leverages Vector3.Reflect().
    /// </summary>
    /// <param name="collisionNormal">
    /// The surface normal at the point of collision.
    /// </param>
    private void Bounce(Vector3 collisionNormal)
    {
        rb.velocity = Vector3.Reflect(previousVelocity.normalized, collisionNormal) * Speed;
    }

    public void SetColourAndMaterial(Colours colour, Material material)
    {
        Colour = colour;
        GetComponent<MeshRenderer>().material = material;
    }
}
