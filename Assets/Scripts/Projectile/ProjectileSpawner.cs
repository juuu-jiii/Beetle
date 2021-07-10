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

    public GameObject Shoot(Colours projectileColour, Material projectileMaterial)
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

        Marble projectileScript = projectile.GetComponent<Marble>();
        //Debug.Log(projectileScript.Rb);
        projectileScript.Rb.velocity = transform.TransformDirection(
            direction * projectileScript.speed);

        projectileScript.Colour = projectileColour;
        projectileScript.GetComponent<MeshRenderer>().material = projectileMaterial;

        return projectile;
    }
}
