using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private Dictionary<Colours, int> sceneColourTracker;
    [SerializeField]
    private Material[] marbleMaterials;
    [SerializeField]
    private GameObject[] spawnPoints;
    [SerializeField]
    private int waves;
    private int currentWave;
    [SerializeField]
    private int waveMarbleCount;
    public bool WaveSpawningInProgress { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        currentWave = 0;
        WaveSpawningInProgress = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Generates a random number corresponding to an index in marbleMaterials.
    /// </summary>
    /// <returns>
    /// The result of the random number generation.
    /// </returns>
    public int GenerateMarbleColour()
    {
        return Random.Range(0, 0/*marbleMaterials.Length*/);
    }

    public Material GetMarbleMaterial(int index)
    {
        return marbleMaterials[index];
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
    /// <param name="pause">
    /// How long to pause for before spawning the first marble.
    /// NOTE: due to initial overhead causing lag, use values greater than 0f.
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
        float pause,
        float interval,
        List<GameObject> marbles)
    {
        yield return new WaitForSeconds(pause);

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
            int marbleColour = GenerateMarbleColour();
            marbles.Add(
                marbleSpawner.Spawn(
                    (Colours)marbleColour,
                    marbleMaterials[marbleColour]));

            yield return new WaitForSeconds(interval);
        }

        WaveSpawningInProgress = false;
    }

    public void SpawnWave(List<GameObject> marbles)
    {
        // Wave complete - spawn the next.
        if (currentWave < waves)
        {
            currentWave++;
            Debug.Log("Starting wave " + currentWave);

            WaveSpawningInProgress = true;

            // Leverage coroutines to spawn marbles at regular intervals.
            foreach (GameObject spawnPoint in spawnPoints)
                StartCoroutine(
                    SpawnMarbleInterval(
                        spawnPoint.GetComponent<MarbleSpawner>(),
                        waveMarbleCount,
                        3f,
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
}

// TODO LATER: maybe put pooling logic here
// TODO: will all spawning code be maintained here, then?
//      Would this apply to projectiles AND marbles? Or would this be
//      kept separate? Keep pooling logic in mind