using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject projectileTemplate;
    private Vector3 target;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Instantiates a marble at the position of this spawner, and initialises
    /// its velocity in the direction of the cursor.
    /// </summary>
    /// <param name="projectileColour">
    /// The colour to be assigned to this projectile.
    /// </param>
    /// <param name="materialColour">
    /// The material to be applied to this projectile.
    /// </param>
    /// <returns>
    /// A reference to the projectile that is instantiated.
    /// </returns>
    public GameObject Shoot(Colours projectileColour, Color materialColour)
    {
        // Input.mousePosition's z-value is always 0. Set it to reflect the
        // Camera's height from the ground for accurate tracking of mouse
        // click positions.
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;

        // Input.mousePosition returns data in terms of screen coordinates.
        // Comvert this to world coordinates.
        target = Camera.main.ScreenToWorldPoint(mousePos);

        // The direction to move in is the difference between the positions
        // of target and spawnPoint.
        //
        // The result is normalised so that it can later be multiplied
        // accordingly by a speed value.
        Vector3 direction = (target - transform.position).normalized;

        GameObject projectile = Instantiate(
            projectileTemplate,
            transform.position,
            projectileTemplate.transform.rotation);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        //Debug.Log(projectileScript.Rb);

        // Set spawned projectile's initial velocity and colour
        // (via its material).
        projectileScript.Rb.velocity = transform.TransformDirection(
            direction * projectileScript.speed);
        projectileScript.Colour = projectileColour;
        projectileScript.GetComponent<MeshRenderer>().material.color = materialColour;

        // Live projectiles are identified using emissions.
        Color projectileMaterialColour = projectileScript.GetComponent<MeshRenderer>().material.color;
        projectileScript.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        projectileScript.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", projectileMaterialColour);

        return projectile;
    }
}
