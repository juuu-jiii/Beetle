using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> marbles;
    [SerializeField]
    private GameObject[] spawnPoints;
    [SerializeField]
    private Material[] marbleMaterials;
    [SerializeField]
    private GameObject player;
    private Cannon playerScript;
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
        playerScript = player.GetComponent<Cannon>();
        projectileSpawnerScript = projectileSpawner.GetComponent<ProjectileSpawner>();

        // Listen for MarbleMatch and GameOver events upon game start.
        EventManager.StartListening(Events.ProjectileMarbleMatch, ClearMatches);
        EventManager.StartListening(Events.ProjectileProjectileMatch, ClearMatches);
        EventManager.StartListening(Events.GameOver, HandleGameOver);

        // Leverage coroutines to spawn marbles at regular intervals.
        foreach (GameObject spawnPoint in spawnPoints)
            StartCoroutine(SpawnMarbleInterval(spawnPoint.GetComponent<MarbleSpawner>(), 5, 0.25f));
    }

    // Player movement is handled in FixedUpdate() since physics are involved.
    private void FixedUpdate()
    {
        // Check playerScript.Movable to prevent player input while respawning.
        if (playerScript.Movable)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                playerScript.StrafeLeft();

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                playerScript.StrafeRight();

            // TODO REMOVE: debugging
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                playerScript.StrafeUp();

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                playerScript.StrafeDown();
        }
    }

    // Player shooting is handled in Update() since responsiveness is important.
    //
    // Projectile physics are handled in their respective scripts'
    // FixedUpdate() methods.
    private void Update()
    {
        if (playerScript.Movable && Input.GetMouseButtonDown(0))
        {
            // Pass in a randomly-selected colour and its corresponding material
            // within GameManager to ensure array data is properly encapsulated.
            int marbleColour = GenerateMarbleColour();
            marbles.Add(
                projectileSpawnerScript.Shoot(
                    (Colours)marbleColour,
                    marbleMaterials[marbleColour]));
        }
    }

    /// <summary>
    /// Removes and destroys matched marbles from the Scene. Invoked as part
    /// of Events.ProjectileMarbleMatch and Events.ProjectileProjectileMatch.
    /// </summary>
    private void ClearMatches()
    {
        // Loop through marbles and remove those that have been matched.
        // Optimise program by performing this only during event invocation, as
        // opposed to each update frame.
        for (int i = marbles.Count - 1; i >= 0; i--)
        {
            if (marbles[i].GetComponent<Marble>().Matched)
            {
                GameObject remove = marbles[i];
                marbles.RemoveAt(i);
                Destroy(remove);
            }
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
        return Random.Range(0, 0/*marbleMaterials.Length*/);
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
    private IEnumerator SpawnMarbleInterval(
        MarbleSpawner marbleSpawner, 
        int repeats, 
        float interval)
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
                    marbleMaterials[marbleColour]));
        }
    }

    private void HandleGameOver()
    {
        // TODO LATER: implement game over logic
        Debug.Log("No more lives left - game over!");
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