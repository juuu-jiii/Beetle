using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: add marbles to array
    // account for both wave-spawned and projectiles
    [SerializeField]
    private List<GameObject> marbles;
    [SerializeField]
    private GameObject[] spawnPoints;
    [SerializeField]
    private Material[] materials;
    [SerializeField]
    private GameObject player;
    private CannonMovement playerScript;
    [SerializeField]
    private GameObject projectileSpawner;
    private ProjectileSpawner projectileSpawnerScript;
    //[SerializeField]
    //private int waves;
    //[SerializeField]
    //private int waveMarbleCount;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<CannonMovement>();
        projectileSpawnerScript = projectileSpawner.GetComponent<ProjectileSpawner>();

        // Listen for MarbleMatch events upon game start.
        EventManager.StartListening(Events.MarbleMatch, HandleCollision);

        // Leverage coroutines to spawn marbles at regular intervals.
        foreach (GameObject spawnPoint in spawnPoints)
            StartCoroutine(SpawnMarbleInterval(spawnPoint.GetComponent<MarbleSpawner>(), 1, 0.25f));
    }

    // Player movement is handled in FixedUpdate() since physics are involved.
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            playerScript.StrafeLeft();

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            playerScript.StrafeRight();
    }

    // Player shooting is handled in Update() since responsiveness is important.
    //
    // Projectile physics are handled in their respective scripts'
    // FixedUpdate() methods.
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Pass in a randomly-selected colour and its corresponding material
            // within GameManager to ensure array data is properly encapsulated.
            int marbleColour = GenerateMarbleColour();
            marbles.Add(
                projectileSpawnerScript.Shoot(
                    (Colours)marbleColour,
                    materials[marbleColour]));
        }
    }

    /// <summary>
    /// Removes the specified marbles from the Scene.
    /// </summary>
    /// <param name="collider1">
    /// First marble to remove from the Scene.
    /// </param>
    /// <param name="collider2">
    /// Second marble to remove from the Scene.
    /// </param>
    void HandleCollision(GameObject collider1, GameObject collider2)
    {
        if (marbles.Contains(collider1) && marbles.Contains(collider2))
        {
            marbles.Remove(collider1);
            marbles.Remove(collider2);
            Destroy(collider1);
            Destroy(collider2);
            // TODO LATER: increment score - maybe move this into another class
            // (scoring and combos can be handled within)
            //
            // Call that method using the event system.
        }
    }

    /// <summary>
    /// Generates a random number corresponding to an index in materials.
    /// </summary>
    /// <returns>
    /// The result of the random number generation.
    /// </returns>
    private int GenerateMarbleColour()
    {
        return Random.Range(0, /*0*/materials.Length);
    }

    /// <summary>
    /// Uses a coroutine to call marbleSpawner.SpawnMarble() at regular,
    /// specified intervals to spawn marbles a given number of times.
    /// </summary>
    /// <param name="marbleSpawner">
    /// Reference to a MarbleSpawner script.
    /// </param>
    /// <param name="repeats">
    /// The number of marbles to spawn.
    /// </param>
    /// <param name="interval">
    /// The frequency at which to spawn marbles.
    /// </param>
    /// <returns>
    /// Object of type IEnumerator - required for coroutine to work.
    /// </returns>
    IEnumerator SpawnMarbleInterval(MarbleSpawner marbleSpawner, int repeats, float interval)
    {
        for (int i = 0; i < repeats; i++)
        {
            //List.Add(marbleSpawner.SpawnMarble());
            //GameObject marble = marbleSpawner.SpawnMarble();

            //MarbleMovement marbleScript = marble.GetComponent<MarbleMovement>();
            //Debug.Log(marbleScript.rb);
            //marbleScript.rb.velocity = marble.transform.position * marbleScript.speed;

            //marbles.Add(marble);
            
            // Note: yield first for now, since POC requires initial wave to be
            // called on Start(). Without the pause, dependencies i.e. the
            // RigidBody are not loaded yet, and NullReferenceExceptions get
            // thrown.
            // TODO LATER: Move wave spawning into Update()
            // Make sure that each wave is only spawned once - maybe via
            // conditional checks or something.
            yield return new WaitForSeconds(interval);

            // Pass in a randomly-selected colour and its corresponding material
            // within GameManager to ensure array data is properly encapsulated.
            int marbleColour = GenerateMarbleColour();
            marbles.Add(
                marbleSpawner.Spawn(
                    (Colours)marbleColour,
                    materials[marbleColour]));
        }
    }
}

/*
    Issues with spawning:
    - you cannot spawn in rapid fire - there will be collisions and then the 
        marbles won't move in desired directions. So you should delay the actual
        spawning
    - What happens if, while spawning, the player destroys a marble?
        I was thinking of making spawning a 2-step process. In each spawner's
        script, a List<GameObject> of to-be-spawned marbles will be "buffered".
        In the script's Update(), new marbles will be instantiated at regular
        time intervals. Each time an instantiation occurs, the corresponding
        marble will be removed from the buffer and added to the GameManager's
        "master" List.
 */