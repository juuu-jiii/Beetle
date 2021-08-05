using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all game logic.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private Cannon playerScript;
    [SerializeField]
    private GameObject spawnManager;
    private SpawnManager spawnManagerScript;
    [SerializeField]
    private GameObject materialsManager;
    private MaterialsManager materialsManagerScript;

    /// <summary>
    /// Master list of all marbles in the arena.
    /// </summary>
    private List<GameObject> marbles;

    //[SerializeField]
    //private GameObject[] spawnPoints;
    //[SerializeField]
    //private Material[] marbleMaterials;
    //[SerializeField]
    //private GameObject projectileSpawner;
    //private ProjectileSpawner projectileSpawnerScript;
    //[SerializeField]
    //private int waves;
    //private int currentWave;
    //[SerializeField]
    //private int waveMarbleCount;
    //private bool waveSpawningInProgress;

    // Start is called before the first frame update
    void Start()
    {
        marbles = new List<GameObject>();

        playerScript = player.GetComponent<Cannon>();
        spawnManagerScript = spawnManager.GetComponent<SpawnManager>();
        materialsManagerScript = materialsManager.GetComponent<MaterialsManager>();

        // Setup event callbacks accordingly.
        EventManager.StartListening(Events.MarbleMatch, ClearMatch);
        EventManager.StartListening(Events.ProjectileMatch, ClearMatch);
        EventManager.StartListening(Events.GameOver, HandleGameOver);
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
        // Do not let the player shoot during the brief period between waves
        // and while they are disabled.
        if (playerScript.Movable 
            && !spawnManagerScript.InBetweenWaves 
            && Input.GetMouseButtonDown(0))
        {
            // Update marbleColourCountDict in SpawnManager.
            spawnManagerScript.IncrementMarbleColourCount(playerScript.NextColour);

            marbles.Add(playerScript.Shoot());

            // After shooting, set the player's next shot.
            Colours nextColour = spawnManagerScript.ShootMarbleColour();
            playerScript.UpdateNext(
                nextColour,
                materialsManagerScript.GetMaterial(nextColour));
        }

        // When the List is empty, either the wave or the whole level is complete.
        //
        // waveSpawningInProgress prevents multiple waves spawning at once, due
        // to spawning taking a couple frames, leading to there being multiple
        // frames where marbles is empty. For the same reason, it is also used
        // to prevent the win condition from being triggered a couple frames
        // before the last wave actually begins spawning.
        if (marbles.Count == 0 && !spawnManagerScript.WaveSpawningInProgress)
        {
            StartCoroutine(spawnManagerScript.SpawnWave(marbles, 3f));
        }
    }

    /// <summary>
    /// Removes and destroys matched marbles from the Scene and updates
    /// marbleColourCountDict in SpawnManager. Invoked as part of
    /// Events.ProjectileMarbleMatch and Events.ProjectileProjectileMatch.
    /// </summary>
    private void ClearMatch()
    {
        // Loop through marbles and remove those that have been matched.
        // Optimise program by performing this only during event invocation, as
        // opposed to each update frame.
        for (int i = marbles.Count - 1; i >= 0; i--)
        {
            if (marbles[i].GetComponent<Marble>().Matched)
            {
                GameObject remove = marbles[i];

                // Update marbleColourCountDict in SpawnManager.
                spawnManagerScript.DecrementMarbleColourCount(
                    remove.GetComponent<Marble>().Colour);

                marbles.RemoveAt(i);
                Destroy(remove);
            }
        }
    }

    private void HandleGameOver()
    {
        // TODO LATER: implement game over logic
        Debug.Log("No more lives left - game over!");
    }
}