using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coordinates the spawning of all marbles within a level.
/// </summary>
public class SpawnManager : MonoBehaviour
{    
    /// <summary>
    /// Array of references to all spawn points in the arena.
    /// </summary>
    [SerializeField]
    private GameObject[] spawnPoints;

    /// <summary>
    /// Counts of marbles to be spawned each wave.
    /// </summary>
    [SerializeField]
    private int[] waveMarbleCounts;

    [SerializeField]
    private GameObject player;
    private Cannon playerScript;

    [SerializeField]
    private GameObject materialsManager;
    private MaterialsManager materialsManagerScript;

    /// <summary>
    /// Tracks whether a wave is currently spawning.
    /// </summary>
    public bool WaveSpawningInProgress { get; private set; }

    /// <summary>
    /// Tracks whether the game is currently in betweem waves.
    /// </summary>
    public bool InBetweenWaves { get; private set; }

    /// <summary>
    /// Keeps a live count of the number of each coloured destructible in the
    /// arena.
    /// </summary>
    private Dictionary<Colours, int> destructibleColourCountDict;

    /// <summary>
    /// The current wave in the level.
    /// </summary>
    public int currentWave { get; private set; }

    /// <summary>
    /// Master list of all colours lined up for the next wave. Used for setting
    /// the first colour to be shot each wave.
    /// </summary>
    private List<Colours> bufferedColoursMaster;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<Cannon>();
        materialsManagerScript = materialsManager.GetComponent<MaterialsManager>();

        // Start at -1 because currentWave is zero-based.
        currentWave = -1;

        WaveSpawningInProgress = false;
        InBetweenWaves = true;
        destructibleColourCountDict = new Dictionary<Colours, int>();
        bufferedColoursMaster = new List<Colours>();

        // Upon game start, buffer a set of marble colours at all spawn points.
        //BufferSpawnPoints();

        // Setup event callbacks accordingly.
        EventManager.Instance.StartListening(Events.MarbleMatch, ValidatePlayerNext);
        EventManager.Instance.StartListening(Events.ProjectileMatch, ValidatePlayerNext);
        EventManager.Instance.StartListening(Events.TargetMatch, ValidatePlayerNext);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Clears and buffers each spawner, updating bufferedColoursMaster
    /// accordingly.
    /// </summary>
    private void BufferSpawnPoints()
    {
        bufferedColoursMaster.Clear();

        foreach (GameObject spawnPoint in spawnPoints)
        {
            MarbleSpawner spawnPointScript = spawnPoint.GetComponent<MarbleSpawner>();
            spawnPointScript.ClearBuffer();

            for (int i = 0; i < waveMarbleCounts[currentWave]; i++)
            {
                Colours colour = SpawnMarbleColour();

                // Buffer the appropriate spawner, and also update
                // bufferedColoursMaster.
                bufferedColoursMaster.Add(colour);
                spawnPointScript.BufferColour(colour);
            }
        }
    }

    /// <summary>
    /// Generates a random marble colour. Called when a wave is spawned.
    /// </summary>
    /// <returns>
    /// The result of the random colour generation.
    /// </returns>
    public Colours SpawnMarbleColour()
    {
        return (Colours)Random.Range(0, /*0*/materialsManagerScript.EntryCount);
    }

    /// <summary>
    /// Generates a random marble colour based on those in play. Called when
    /// the player shoots a marble.
    /// </summary>
    /// <returns>
    /// The result of the random colour generation.
    /// </returns>
    public Colours ShootMarbleColour()
    {
        int colourIndex = /*0;*/ Random.Range(0, destructibleColourCountDict.Count);
        int i = 0;
        // Default colour, because the compiler does not know that the
        // following loop will always assign a colour to the variable.
        Colours shootColour = Colours.Red;

        // Fake an LCV and loop through marbleColourCountDict.
        // The keys are unordered, but that does not matter since a random
        // colour is to be returned anyway.
        foreach (KeyValuePair<Colours, int> entry in destructibleColourCountDict)
        {
            Debug.Log("count of destructibleColourCountDict = " + destructibleColourCountDict.Count);
            Debug.Log("running ShootMarbleColour loop");
            if (i == colourIndex)
            {
                shootColour = entry.Key;
                break;
            }
            else i++;
        }

        return shootColour;
    }

    /// <summary>
    /// Increments the specified destructible colour's count.
    /// </summary>
    /// <param name="colour">
    /// The destructible colour whose count is to be updated.
    /// </param>
    public void IncrementDestructibleColourCount(Colours colour)
    {
        if (!destructibleColourCountDict.ContainsKey(colour))
            destructibleColourCountDict.Add(colour, 0);

        destructibleColourCountDict[colour]++;
    }

    /// <summary>
    /// Decrements the specified destructible colour's count, and removes it from
    /// destructibleColourCountDict if the new count is 0, making that colour out
    /// of play.
    /// </summary>
    /// <param name="colour">
    /// The destructible colour whose count is to be updated.
    /// </param>
    public void DecrementDestructibleColourCount(Colours colour)
    {
        if (destructibleColourCountDict.ContainsKey(colour) && --destructibleColourCountDict[colour] == 0)
            destructibleColourCountDict.Remove(colour);
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
    /// <param name="marbles">
    /// Reference to marbles List in GameManager.
    /// </param>
    /// <returns>
    /// Object of type IEnumerator - required for coroutine to work.
    /// </returns>
    private IEnumerator SpawnMarbleInterval(
        MarbleSpawner marbleSpawner,
        int repeats,
        float interval,
        List<GameObject> marbles)
    {
        InBetweenWaves = false;

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

            // Pass in a randomly-selected colour and its corresponding material
            // within GameManager to ensure array data is properly encapsulated.

            // Peek at the next colour from the queue, and use it to obtain the
            // corresponding material from marbleMaterials.
            Colours nextColour = marbleSpawner.Next;

            // Update marbles as each new marble is spawned.
            marbles.Add(marbleSpawner.Spawn());

            // Signal to GameManager that marble speeds might need adjusting.
            EventManager.Instance.TriggerEvent(Events.MarbleSpawn);

            // Update sceneColourTrackerDict as more marbles are spawned.
            //if (!destructibleColourCountDict.ContainsKey(nextColour))
            //    destructibleColourCountDict.Add(nextColour, 1);
            //else
            //    destructibleColourCountDict[nextColour]++;
            IncrementDestructibleColourCount(nextColour);

            yield return new WaitForSeconds(interval);
        }

        WaveSpawningInProgress = false;
    }

    /// <summary>
    /// Spawns the next wave of marbles, updating GameManager's marbles
    /// List accordingly.
    /// </summary>
    /// <param name="marbles">
    /// Master list of marbles in the arena, maintained by GameManager.
    /// </param>
    /// <param name="pause">
    /// How long to pause for before spawning the first marble.
    /// NOTE: due to initial overhead causing lag, use values greater than 0f.
    /// </param>
    /// <returns>
    /// Object of type IEnumerator - required for coroutine to work.
    /// </returns>
    public IEnumerator SpawnWave(List<GameObject> marbles, float pause)
    {
        // Wave complete - spawn the next.
        // Take length of array - 1 because currentWave is zero-based.
        if (currentWave < waveMarbleCounts.Length - 1)
        {
            currentWave++;
            BufferSpawnPoints();
            Debug.Log("Starting wave " + currentWave);

            // Update the player's first shot of the wave with a
            // randomly-selected colour from bufferedColoursMaster.
            Colours firstShotColour = 
                bufferedColoursMaster[Random.Range(0, bufferedColoursMaster.Count)];
            playerScript.UpdateNext(
                firstShotColour,
                materialsManagerScript.GetMaterial(firstShotColour));

            InBetweenWaves = true;
            WaveSpawningInProgress = true;

            yield return new WaitForSeconds(pause);

            // Leverage coroutines to spawn marbles at regular intervals.
            foreach (GameObject spawnPoint in spawnPoints)
                StartCoroutine(
                    SpawnMarbleInterval(
                        spawnPoint.GetComponent<MarbleSpawner>(),
                        waveMarbleCounts[currentWave],
                        0.25f,
                        marbles));
        }
        // Level complete.
        else
        {
            EventManager.Instance.TriggerEvent(Events.LevelComplete);
        }
    }

    /// <summary>
    /// Checks the next shot's colour against the marbles in play, adjusting it
    /// if necessary to ensure only valid colours are supplied. Invoked
    /// alongside both MarbleMatch and ProjectileMatch events.
    /// </summary>
    public void ValidatePlayerNext()
    {
        if (!destructibleColourCountDict.ContainsKey(playerScript.NextColour))
        {
            Colours nextColour = ShootMarbleColour();
            playerScript.UpdateNext(
                nextColour,
                materialsManagerScript.GetMaterial(nextColour));
        }
    }
}

// TODO LATER: maybe put pooling logic here
// TODO: will all spawning code be maintained here, then?
//      Would this apply to projectiles AND marbles? Or would this be
//      kept separate? Keep pooling logic in mind
// TODO LATER: make buffering asynchronous