using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;
    private bool resetting;
    private Animation anim;
    private Transform projectileSpawnerTransform;
    private Aim aimScript;
    [SerializeField]
    private float speed;
    [SerializeField]
    private GameObject projectileTemplate;
    public bool Movable { get; private set; }
    public int Lives { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animation>();
        Movable = true;
        Lives = 3;
        projectileSpawnerTransform = transform.GetChild(0);

        // Projectile Spawn Point is a child of this GameObject.
        aimScript = transform.GetChild(0).gameObject.GetComponent<Aim>();
    }

    // Handle movement physics in FixedUpdate().
    void FixedUpdate()
    {
        // Reset each frame so any movement is updated correctly.
        velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Logic applied when colliding with a Marble or Projectile.
        if (collision.gameObject.CompareTag("Marble") || collision.gameObject.CompareTag("Projectile"))
        {
            // Use bool resetting to ensure player does not lose multiple lives
            // if caught in multiple simultaneous collisions.
            if (!resetting)
            {
                resetting = true;
                // Decrement lives, invoking Game Over event if they run out.
                // Otherwise, the reset animation is played.
                if (--Lives == 0) EventManager.TriggerEvent(Events.GameOver);

                Debug.Log("Lives left: " + Lives);
                StartCoroutine(PlayResetAnimation(3f, 5f, 0.7f));
            }
        }
    }

    /// <summary>
    /// Hides and disables the player for a specified number of seconds, before
    /// re-displaying them, greyed-out, and invincible, for a further given
    /// number of seconds. At the halfway point of this invincibility period,
    /// flashes the player on the screen, while keeping them greyed-out.
    /// Restores regular player appearance once invincibility runs out.
    /// </summary>
    /// <param name="durationDisabled">
    /// Total amount of time player is to be hidden and disabled for, in seconds.
    /// </param>
    /// <param name="durationInvincible">
    /// Total amount of time player is to be invincible for, in seconds.
    /// </param>
    /// <param name="alpha">
    /// Alpha factor - the degree at which the player appears greyed-out.
    /// </param>
    /// <returns>
    /// Object of type IEnumerator - required for coroutine to work.
    /// </returns>
    private IEnumerator PlayResetAnimation(
        float durationDisabled,
        float durationInvincible,
        float alpha)
    {
        // Configure greyed-out material colour.
        Color materialColourGreyed = GetComponent<MeshRenderer>().material.color;
        materialColourGreyed.a = alpha;

        // SetActive() cannot be used because coroutines are stopped when a
        // GameObject is inactive. Instead, isKinematic, MeshRenderer.enabled,
        // and Movable (used in GameManager to prevent movement) are all
        // set to false to achieve a similar effect.

        // Prevent physics from being applied to player while disabled.
        rb.isKinematic = true;

        // Disallow player input in GameManager.
        Movable = false;

        // Hide player
        GetComponent<MeshRenderer>().enabled = false;

        // Ignore collisions from marbles by moving player to a different
        // collision layer.
        this.gameObject.layer = LayerMask.NameToLayer("Greyed-out");

        // Disable player for a fixed duration.
        yield return new WaitForSeconds(durationDisabled);

        // Player is now visible and enabled, albeit greyed-out.
        // Offer a couple seconds of invincibility during this time.
        GetComponent<MeshRenderer>().enabled = true;
        rb.isKinematic = false;
        Movable = true;
        Extensions.ChangeRenderMode(GetComponent<MeshRenderer>().material, RenderingModes.Fade);
        GetComponent<MeshRenderer>().material.color = materialColourGreyed;
        yield return new WaitForSeconds(durationInvincible / 2);

        // Invincibility running out i.e. 1/2 time remaining - flash player
        // (emulates iFrames).
        anim.Play("Fade");
        yield return new WaitForSeconds(durationInvincible / 2);
        anim.Stop("Fade");

        // Invincibility period over; stop flashing Cannon and reinstate collisions.
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        Extensions.ChangeRenderMode(GetComponent<MeshRenderer>().material, RenderingModes.Opaque);
        resetting = false;
    }

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

    /// <summary>
    /// Instantiates a marble at the position of this spawner, and initialises
    /// its velocity in the direction of the cursor.
    /// </summary>
    /// <param name="projectileColour">
    /// The colour to be assigned to this projectile.
    /// </param>
    /// <param name="projectileMaterial">
    /// The material to be applied to this projectile.
    /// </param>
    /// <returns>
    /// A reference to the projectile that is instantiated.
    /// </returns>
    public GameObject Shoot(Colours projectileColour, Material projectileMaterial)
    {
        GameObject projectile = Instantiate(
            projectileTemplate,
            projectileSpawnerTransform.position,
            projectileTemplate.transform.rotation);

        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // Set spawned projectile's initial velocity and colour
        // (via its material).
        projectileScript.Rb.velocity = transform.TransformDirection(
            aimScript.AimDirection * projectileScript.speed);
        projectileScript.Colour = projectileColour;
        projectileScript.GetComponent<MeshRenderer>().material = projectileMaterial;

        // Live projectiles are identified using emissions.
        Color projectileMaterialColour = projectileScript.GetComponent<MeshRenderer>().material.color;
        projectileScript.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        projectileScript.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", projectileMaterialColour);

        return projectile;
    }
}
