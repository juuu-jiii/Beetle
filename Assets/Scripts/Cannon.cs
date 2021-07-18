using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;
    [SerializeField]
    private float speed;
    public bool GreyedOut { get; set; }
    //private bool ignoreCollisions;
    //private int collisionCounter;
    //private int prevCollisionCounter;
    private Animation anim;
    public bool Movable { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Movable = true;
        
        //// Listen for LifeLost events upon game start.
        //EventManager.StartListening(Events.LifeLost, PlayResetSequence);

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animation>();
    }

    // Handle physics in FixedUpdate().
    void FixedUpdate()
    {
        // Reset each frame so any movement is updated correctly.
        velocity = Vector3.zero;
    }

    // Handle non-physics logic in Update().
    private void Update()
    {
        //Debug.Log(string.Format("GreyedOut = {0} collisionCounter = {1}", GreyedOut, collisionCounter));

        //if (!GreyedOut && collisionCounter == 0) 
        //    ignoreCollisions = false;
        //if (!GreyedOut)
        //    this.gameObject.layer = LayerMask.NameToLayer("Default");

        //prevCollisionCounter = collisionCounter;
            
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Marble") || collision.gameObject.CompareTag("Projectile"))
        {
            StartCoroutine(PlayResetSequence(3f, 5f, 0.4f));
            EventManager.TriggerEvent(Events.LifeLost);
        }

        //if (collision.gameObject.tag == "Marble")
        //    StartCoroutine(PlayResetSequence(3f, 5f, 0.4f));

        // Invoke event

        //if (collision.gameObject.tag == "Marble")
        //{
        //    collisionCounter++;

        //    if (!GreyedOut && ignoreCollisions)
        //        Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider, true);
        //}

        //if (ignoreCollisions && collision.gameObject.tag == "Marble")
        //    Physics.IgnoreCollision()

    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("collision exited");

        //if (collision.gameObject.tag == "Marble")
        //{
        //    collisionCounter--;

        //    if (!GreyedOut && ignoreCollisions)
        //    {
        //        Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider, false);

        //        if (collisionCounter == 0) ignoreCollisions = false;
        //    }
        //}
    }


    // TODO: refactor to disallow unnecessary params

    /// <summary>
    /// Wrapper for the AnimateReset() coroutine so it can be invoked when
    /// events are triggered.
    /// </summary>
    private IEnumerator PlayResetSequence(
        float durationInactive,
        float durationInvincible,
        float alpha)
    {
        // Configure greyed-out material colour.
        Color materialColourGreyed = GetComponent<MeshRenderer>().material.color;
        materialColourGreyed.a = alpha;

        // Cannon is inactive for a fixed duration.
        rb.isKinematic = true;
        Movable = false;
        // Hide Cannon
        GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.layer = LayerMask.NameToLayer("Greyed-out");
        yield return new WaitForSeconds(durationInactive);
        //this.gameObject.SetActive(true);
        GetComponent<MeshRenderer>().enabled = true;
        rb.isKinematic = false;
        Movable = true;
        //Debug.Log("setting active");

        // Offer a couple seconds of invincibility, where Cannon is greyed out.
        // While Cannon is greyed out, move to different layer to ignore colllisions.
        //GreyedOut = true;
        Extensions.ChangeRenderMode(GetComponent<MeshRenderer>().material, RenderModes.Transparent);
        GetComponent<MeshRenderer>().material.color = materialColourGreyed;
        yield return new WaitForSeconds(durationInvincible / 2);

        // Invincibility running out i.e. 1/2 time remaining - flash Cannon
        // (resembles iFrames).
        anim.Play("Fade");
        yield return new WaitForSeconds(durationInvincible / 2);
        anim.Stop("Fade");

        // Invincibility period over; stop flashing Cannon and reinstate collisions.
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        //materialColour.a = 1f;
        //GetComponent<MeshRenderer>().material.color = materialColour;
        Extensions.ChangeRenderMode(GetComponent<MeshRenderer>().material, RenderModes.Opaque);
        //GreyedOut = false;

        // wait a number of seconds (greyed out)
        // play animation
        // wait a number of seconds (flashing)
        // stop animation
        // make sure to change the material back!!!
        // reinstate collisions

        //if (GreyedOut)
        //{
        //    collisionCounter = 0;
        //    ignoreCollisions = true;
        //    this.gameObject.layer = LayerMask.NameToLayer("Greyed-out");
        //    StartCoroutine(PlayResetSequence(
        //        3f,
        //        false,
        //        RenderModes.Opaque,
        //        GetComponent<MeshRenderer>().material.color,
        //        1f));
        //}
    }

    //public IEnumerator FadeInOut(float fadeDuration, float totalDuration)
    //{
    //    float t = 0;

    //    while (t < totalDuration)
    //    {

    //    }
    //}

    private IEnumerator FadeMaterial(Material material, float finalOpacity, float fadeDuration)
    {
        Color colour = material.color;
        float initialOpacity = colour.a;

        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float lerpFactor = Mathf.Clamp01(t / fadeDuration);

            colour.a = Mathf.Lerp(initialOpacity, finalOpacity, lerpFactor);

            material.color = colour;

            yield return null;
        }

        // Start another coroutine here with conditional to alternate. Stop the coroutine
        // when the total duration has elapsed in the caller.
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Marble")
    //    {

    //    }
    //}

    /// <summary>
    /// Move the player left.
    /// </summary>
    public void StrafeLeft()
    {
        velocity = Vector3.left * speed;

        // AddForce() cannot be used - movement must be immediate.
        transform.position += velocity;
    }

    /// <summary>
    /// Move the player right.
    /// </summary>
    public void StrafeRight()
    {
        velocity = Vector3.right * speed;

        // AddForce() cannot be used - movement must be immediate.
        transform.position += velocity;
    }

    // TODO REMOVE: debugging
    public void StrafeUp()
    {
        velocity = Vector3.forward * speed;

        // AddForce() cannot be used - movement must be immediate.
        transform.position += velocity;
    }

    public void StrafeDown()
    {
        velocity = Vector3.back * speed;

        // AddForce() cannot be used - movement must be immediate.
        transform.position += velocity;
    }
}
