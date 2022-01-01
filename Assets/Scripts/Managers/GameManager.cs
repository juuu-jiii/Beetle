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
    [SerializeField]
    private GameObject targetManager;
    private TargetManager targetManagerScript;

    /// <summary>
    /// Minimum speed at which a marble can travel after being subjected to
    /// slowdowns.
    /// </summary>
    [SerializeField]
    private float marbleMinSpeed;

    /// <summary>
    /// The number of marbles at which to call UpdateDestructibleColourCountDict
    /// with UpdateAction Remove in TargetManager.
    /// </summary>
    [SerializeField]
    private int targetCutoffThreshold;

    private bool cutoffThisWave = false;

    /// <summary>
    /// Master list of all marbles in the arena.
    /// </summary>
    private List<GameObject> marbles;

    [SerializeField]
    int speedChangeThreshold; // = 4

    [SerializeField]
    private GameObject marbleTemplate;
    private Marble marbleTemplateScript;

    /// <summary>
    /// Maximum speed at which a marble can travel after being suggested to
    /// speedups.
    /// </summary>
    private float marbleMaxSpeed;

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

    private void Awake()
    {
        // Setup event callbacks accordingly:

        // Callbacks that clear marbles:
        EventManager.StartListening(Events.MarbleMatch, ClearMarbleMatch);
        EventManager.StartListening(Events.ProjectileMatch, ClearMarbleMatch);
        EventManager.StartListening(Events.TargetMatch, ClearMarbleMatch);

        // Callbacks that adjust marble speed:
        EventManager.StartListening(Events.MarbleMatch, AdjustMarbleSpeed);
        EventManager.StartListening(Events.ProjectileMatch, AdjustMarbleSpeed);
        EventManager.StartListening(Events.MarbleSpawn, AdjustMarbleSpeed);

        // Callback handling game over and level complete states:
        EventManager.StartListening(Events.GameOver, GameOver);
        EventManager.StartListening(Events.LevelComplete, LevelComplete);
    }

    // Start is called before the first frame update
    void Start()
    {
        marbles = new List<GameObject>();
        marbleTemplateScript = marbleTemplate.GetComponent<Marble>();
        marbleMaxSpeed = marbleTemplateScript.TopSpeed;

        playerScript = player.GetComponent<Cannon>();
        spawnManagerScript = spawnManager.GetComponent<SpawnManager>();
        materialsManagerScript = materialsManager.GetComponent<MaterialsManager>();
        targetManagerScript = targetManager.GetComponent<TargetManager>();
    }

    // Player movement is handled in FixedUpdate() since physics are involved.
    // TODO: Use Input.GetAxis()
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
            // Update destructibleColourCountDict in SpawnManager.
            spawnManagerScript.IncrementDestructibleColourCount(playerScript.NextColour);

            marbles.Add(playerScript.Shoot(marbles[0].GetComponent<Marble>().Speed));

            // Check and adjust marble speeds as appropriate.
            AdjustMarbleSpeed();

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

            // Re-populate destructibleColourCountDict with Target colours at
            // the beginning of each wave.
            targetManagerScript.UpdateDestructibleColourCountDict(UpdateAction.Add);
            cutoffThisWave = false;
        }

        // TODO REMOVE: debugging
        if (Input.GetKeyDown(KeyCode.Escape))
            StateManager.Instance.SetState(GameStates.Title);
    }

    /// <summary>
    /// Removes and destroys matched marbles from the Scene and updates
    /// destructibleColourCountDict in SpawnManager. Invoked as part of
    /// Events.ProjectileMarbleMatch and Events.ProjectileProjectileMatch.
    /// </summary>
    private void ClearMarbleMatch()
    {
        // Loop through marbles and remove those that have been matched.
        // Optimise program by performing this only during event invocation, as
        // opposed to each update frame.
        for (int i = marbles.Count - 1; i >= 0; i--)
        {
            if (marbles[i].GetComponent<Marble>().Matched)
            {
                GameObject remove = marbles[i];

                // Update destructibleColourCountDict in SpawnManager.
                spawnManagerScript.DecrementDestructibleColourCount(
                    remove.GetComponent<Marble>().Colour);

                marbles.RemoveAt(i);
                Destroy(remove);
            }
        }

        if (marbles.Count <= targetCutoffThreshold && !cutoffThisWave)
        {
            targetManagerScript.UpdateDestructibleColourCountDict(UpdateAction.Remove);
            cutoffThisWave = true;
        }
    }

    /// <summary>
    /// Increases and decreases the speeds of marbles in the arena based on
    /// their numbers.
    /// </summary>
    private void AdjustMarbleSpeed()
    {
        if (marbles.Count > 0)
        {
            // When there are up to 4 marbles on-screen, they travel at maximum speed.
            // For every speedChangeThreshold after that, decrement marbles' speed by 1.
            int speedChangeFactor = marbles.Count <= 4 ?
                0 :
                (marbles.Count - 4) / speedChangeThreshold;

            float currentMarbleSpeed = marbles[0].GetComponent<Marble>().Speed;

            // Marble speeds are updated if they are not equal to the difference
            // between marbleMaxSpeed and speedChangeFactor, provided this new
            // value does not violate min/max speed constraints.
            if (currentMarbleSpeed != marbleMaxSpeed - speedChangeFactor
                && (marbleMaxSpeed - speedChangeFactor >= marbleMinSpeed
                || marbleMinSpeed + speedChangeFactor <= marbleMaxSpeed))
            {
                foreach (GameObject marble in marbles)
                {
                    Marble marbleScript = marble.GetComponent<Marble>();

                    // A distinction between projectiles and marbles is required
                    // here; a projectile's speed while live should not be
                    // altered - only its stale speed.
                    if (marbleScript is Projectile)
                    {
                        Projectile projectileScript = marble.GetComponent<Projectile>();

                        // If projectile is live, adjust its StaleSpeed so it
                        // slows to the new speed after becoming stale.
                        if (!projectileScript.IsStale)
                            projectileScript.StaleSpeed = marbleMaxSpeed - speedChangeFactor;
                        // Otherwise, projectile is stale. Treat it like a
                        // regular marble.
                        else
                            projectileScript.Speed = marbleMaxSpeed - speedChangeFactor;
                    }
                    // In the case of a regular marble, just update its speed.
                    else
                        marbleScript.Speed = marbleMaxSpeed - speedChangeFactor;
                }
            }
        }
    }

    private void LevelComplete()
    {
        Debug.Log("Level complete");
    }

    private void GameOver()
    {
        // TODO LATER: implement game over logic
        Debug.Log("No more lives left - game over!");
        playerScript.Restart();
        StateManager.Instance.SetState(GameStates.GameOver);
    }
}