using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject projectileTemplate;
    private Vector3 target;
    private Aim aimScript;
    
    // Start is called before the first frame update
    void Start()
    {
        aimScript = GetComponent<Aim>();
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
            transform.position,
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
