using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes the properties/behaviours of the Cannon object controlled by the
/// player.
/// </summary>
public class Cannon : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private Rigidbody rb;
    private Vector3 velocity;
    private Animation anim;
    public int Lives { get; private set; }

    /// <summary>
    /// Tracks whether the Player is in the midst of resetting. 
    /// </summary>
    private bool resetting;

    /// <summary>
    /// Reference to Transform component of Projectile Spawn Point child.
    /// </summary>
    private Transform projectileSpawnerTransform;

    /// <summary>
    /// Reference to Aim script of Projectile Spawn Point child.
    /// </summary>
    private Aim aimScript;

    /// <summary>
    /// Colour of the next marble to be shot.
    /// </summary>
    public Colours NextColour { get; private set; }

    /// <summary>
    /// Material of the next marble to be shot.
    /// </summary>
    public Material NextMaterial { get; private set; }

    /// <summary>
    /// Template prefab for projectiles.
    /// </summary>
    [SerializeField]
    private GameObject projectileTemplate;

    /// <summary>
    /// Tracks whether the player has control over the Cannon's movement.
    /// </summary>
    public bool Movable { get; private set; }

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

        // Reset data accordingly.
        EventManager.StartListening(Events.Restart, Restart);
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

        // Hide player and aim guard.
        GetComponent<MeshRenderer>().enabled = false;
        aimScript.AimGuard.enabled = false;

        // Ignore collisions from marbles by moving player to a different
        // collision layer.
        this.gameObject.layer = LayerMask.NameToLayer("Greyed-out");

        // Disable player for a fixed duration.
        yield return new WaitForSeconds(durationDisabled);

        // Player is now visible and enabled, albeit greyed-out.
        // Aim guard is also now visible.
        // Offer a couple seconds of invincibility during this time.
        GetComponent<MeshRenderer>().enabled = true;
        rb.isKinematic = false;
        Movable = true;
        aimScript.AimGuard.enabled = true;
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
    /// its velocity in the direction of the Aim Guard.
    /// </summary>
    /// <returns>
    /// A reference to the projectile that is instantiated.
    /// </returns>
    public GameObject Shoot(float speed)
    {
        GameObject projectile = Instantiate(
            projectileTemplate,
            projectileSpawnerTransform.position,
            projectileTemplate.transform.rotation);

        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // Set spawned projectile's initial velocity and colour
        // (via its material).
        projectileScript.Rb.velocity = transform.TransformDirection(
            aimScript.AimDirection * projectileScript.Speed);
        projectileScript.SetColourAndMaterial(NextColour, NextMaterial);

        // Live projectiles are identified using emissions.
        Color projectileMaterialColour = projectileScript.GetComponent<MeshRenderer>().material.color;
        projectileScript.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        projectileScript.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", projectileMaterialColour);

        // Set stale speed equal to reflect current game state.
        projectileScript.StaleSpeed = speed;

        return projectile;
    }

    /// <summary>
    /// Update the next shot's colour and associated material, alongside the
    /// Aim Guard's colour. Called whenever a shot is made, or a new wave is 
    /// spawned.
    /// </summary>
    /// <param name="nextColour">
    /// The colour of the next shot's marble.
    /// </param>
    /// <param name="nextMaterial">
    /// The material associated with nextColour.
    /// </param>
    public void UpdateNext(Colours nextColour, Material nextMaterial)
    {
        NextColour = nextColour;
        NextMaterial = nextMaterial;

        // Leverage the fact that materials have a color property to also
        // update the LineRenderer's colour accordingly.
        aimScript.AimGuard.startColor = nextMaterial.color;
    }

    public void Restart()
    {
        Lives = 3;
    }

    public void Pause()
    {
        Movable = false;
        aimScript.AimGuard.enabled = false;
        //aimScript.gameObject.SetActive = false;
    }

    public void Unpause()
    {
        // Reactivate the player if they are not in the midst of the resetting
        // sequence.
        if (!resetting)
        {
            Movable = true;
            aimScript.AimGuard.enabled = true;
        }
        //aimScript.gameObject.SetActive = true;
    }
}
