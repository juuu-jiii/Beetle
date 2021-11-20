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
    /// Minimum speed at which a marble must travel after being subjected to
    /// slowdowns.
    /// </summary>
    [SerializeField]
    private float minSpeed;

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
    private float marbleTopSpeed;

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

        // Callback handling game over state:
        EventManager.StartListening(Events.GameOver, HandleGameOver);
    }

    // Start is called before the first frame update
    void Start()
    {
        marbles = new List<GameObject>();
        marbleTemplateScript = marbleTemplate.GetComponent<Marble>();
        marbleTopSpeed = marbleTemplateScript.TopSpeed;

        playerScript = player.GetComponent<Cannon>();
        spawnManagerScript = spawnManager.GetComponent<SpawnManager>();
        materialsManagerScript = materialsManager.GetComponent<MaterialsManager>();
        targetManagerScript = targetManager.GetComponent<TargetManager>();
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

    private void AdjustMarbleSpeed()
    {
        // When there are 4 marbles on-screen, marbles travel at maximum speed.
        // For every 4 after that, decrement marbles' speed by 1.
        int speedChangeFactor = (marbles.Count - 1) / speedChangeThreshold;

        if (marbles.Count > 0)
        {
            float currentMarbleSpeed = marbles[0].GetComponent<Marble>().Speed;

            if (currentMarbleSpeed > marbleTopSpeed - (speedChangeFactor * 3)
                && currentMarbleSpeed > minSpeed) // Limit speed reduction
            {
                foreach (GameObject marble in marbles)
                {
                    // TODO POTENTIAL FIX: downcast to projectile and set
                    // stalespeed if it is live. otherwise just set regular
                    // speed/
                    Debug.Log("Slowing down");
                    Marble marbleScript = marble.GetComponent<Marble>();

                    if (marbleScript is Projectile)
                    {
                        Projectile projectileScript = marble.GetComponent<Projectile>();

                        if (!projectileScript.IsStale)
                        {
                            projectileScript.StaleSpeed = (marbleTopSpeed - speedChangeFactor * 3);
                        }
                        else
                            projectileScript.Speed = (marbleTopSpeed - speedChangeFactor * 3);
                    }
                    else
                        marbleScript.Speed = (marbleTopSpeed - speedChangeFactor * 3);
                }
            }
            //else if (currentMarbleSpeed < marbleTopSpeed - speedChangeFactor)
            //{
            //    foreach (GameObject marble in marbles)
            //    {
            //        Debug.Log("Speeding up");
            //        Marble marbleScript = marble.GetComponent<Marble>();
            //        marbleScript.Speed -= currentMarbleSpeed
            //                            - (marbleTopSpeed - speedChangeFactor);
            //    }
            //}
        }
    }

    private void HandleGameOver()
    {
        // TODO LATER: implement game over logic
        Debug.Log("No more lives left - game over!");
    }
}