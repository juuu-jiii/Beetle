using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    /// <summary>
    /// Array of all possible marble materials that can be applied this level.
    /// </summary>
    [SerializeField]
    private Material[] marbleMaterials;

    /// <summary>
    /// Array of references to all spawn points in the arena.
    /// </summary>
    [SerializeField]
    private GameObject[] spawnPoints;

    /// <summary>
    /// The total number of waves this level.
    /// </summary>
    [SerializeField]
    private int waves;

    /// <summary>
    /// The number of marbles to be spawned each wave.
    /// </summary>
    [SerializeField]
    private int waveMarbleCount;

    [SerializeField]
    private GameObject player;
    private Cannon playerScript;

    /// <summary>
    /// Tracks whether a wave is currently spawning.
    /// </summary>
    public bool WaveSpawningInProgress { get; private set; }
    public bool IsIntermission { get; private set; }

    /// <summary>
    /// Keeps a live count of the number of each coloured marble in the arena.
    /// </summary>
    private Dictionary<Colours, int> marbleColourCountDict;

    /// <summary>
    /// The current wave in the level.
    /// </summary>
    private int currentWave;

    /// <summary>
    /// Master list of all colours lined up for the next wave. Used for setting
    /// the first colour to be shot each wave.
    /// </summary>
    private List<Colours> bufferedColoursMaster;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<Cannon>();

        WaveSpawningInProgress = false;
        IsIntermission = true;
        currentWave = 0;
        marbleColourCountDict = new Dictionary<Colours, int>();
        bufferedColoursMaster = new List<Colours>();

        // Upon game start, buffer a set of marble colours at all spawn points.
        BufferSpawnPoints();

        // Setup event callbacks accordingly.
        EventManager.StartListening(Events.MarbleMatch, ValidatePlayerNext);
        EventManager.StartListening(Events.ProjectileMatch, ValidatePlayerNext);
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

            for (int i = 0; i < waveMarbleCount; i++)
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
        return (Colours)Random.Range(0, /*0*/marbleMaterials.Length);
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
        int colourIndex = Random.Range(0, marbleColourCountDict.Count);
        int i = 0;
        // Default colour, because the compiler does not know that the
        // following loop will always assign a colour to the variable.
        Colours shootColour = Colours.Red;

        // Fake an LCV and loop through marbleColourCountDict.
        // The keys are unordered, but that does not matter since a random
        // colour is to be returned anyway.
        foreach (KeyValuePair<Colours, int> entry in marbleColourCountDict)
        {
            if (i == colourIndex)
            {
                shootColour = entry.Key;
                break;
            }
            else i++;
        }

        return shootColour;
    }

    public Material GetMarbleMaterial(int index)
    {
        return marbleMaterials[index];
    }

    /// <summary>
    /// Increments the specified marble colour's count.
    /// </summary>
    /// <param name="colour">
    /// The marble colour whose count is to be updated.
    /// </param>
    public void IncrementMarbleColourCount(Colours colour)
    {
        if (!marbleColourCountDict.ContainsKey(colour))
            marbleColourCountDict.Add(colour, 0);

        marbleColourCountDict[colour]++;
    }

    /// <summary>
    /// Decrements the specified marble colour's count, and removes it from
    /// marbleColourCountDict if the new count is 0, making that colour out
    /// of play.
    /// </summary>
    /// <param name="colour">
    /// The marble colour whose count is to be updated.
    /// </param>
    public void DecrementMarbleColourCount(Colours colour)
    {
        if (marbleColourCountDict.ContainsKey(colour) && --marbleColourCountDict[colour] == 0)
            marbleColourCountDict.Remove(colour);
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
        IsIntermission = false;

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
            marbles.Add(marbleSpawner.Spawn(marbleMaterials[(int)nextColour]));

            // Update sceneColourTrackerDict as more marbles are spawned.
            if (!marbleColourCountDict.ContainsKey(nextColour))
                marbleColourCountDict.Add(nextColour, 1);
            else
                marbleColourCountDict[nextColour]++;

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
        if (currentWave < waves)
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
                marbleMaterials[(int)firstShotColour]);

            IsIntermission = true;
            WaveSpawningInProgress = true;

            yield return new WaitForSeconds(pause);

            // Leverage coroutines to spawn marbles at regular intervals.
            foreach (GameObject spawnPoint in spawnPoints)
                StartCoroutine(
                    SpawnMarbleInterval(
                        spawnPoint.GetComponent<MarbleSpawner>(),
                        waveMarbleCount,
                        0.25f,
                        marbles));
        }
        // Level complete.
        // TODO LATER: implement level complete logic.
        else
        {
            Debug.Log("Level complete");
        }
    }

    /// <summary>
    /// Checks the next shot's colour against the marbles in play, adjusting it
    /// if necessary to ensure only valid colours are supplied. Invoked
    /// alongside both MarbleMatch and ProjectileMatch events.
    /// </summary>
    public void ValidatePlayerNext()
    {
        if (!marbleColourCountDict.ContainsKey(playerScript.NextColour))
        {
            Colours nextColour = ShootMarbleColour();
            playerScript.UpdateNext(nextColour, marbleMaterials[(int)nextColour]);
        }
    }
}

// TODO LATER: maybe put pooling logic here
// TODO: will all spawning code be maintained here, then?
//      Would this apply to projectiles AND marbles? Or would this be
//      kept separate? Keep pooling logic in mind
// TODO LATER: make buffering asynchronous