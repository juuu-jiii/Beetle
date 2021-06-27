using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject projectileTemplate;
    public Transform spawnPoint;
    private Vector3 target;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
            Vector3 direction = (target - spawnPoint.position).normalized;

            GameObject projectile = MarbleFactory.InstantiateMarble(
                projectileTemplate, 
                spawnPoint.position, 
                projectileTemplate.transform.rotation, 
                false) as GameObject;

            MarbleMovement projectileScript = projectile.GetComponent<MarbleMovement>();
            projectileScript.rb.velocity = spawnPoint.TransformDirection(
                direction * projectileScript.speed);
        }
    }
}
