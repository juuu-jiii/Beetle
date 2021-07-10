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
    private GameObject player;
    [SerializeField]
    private GameObject[] spawnPoints;
    [SerializeField]
    private Material[] materials;
    //[SerializeField]
    //private int waves;
    //[SerializeField]
    //private int waveMarbleCount;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening(Events.MarbleMatch, HandleCollision);

        // TODO: spawn marbles in quick succession using interval
        // Use a coroutine to accomplish this.
        // Spawn only once to show it works, since waves are in a future slice
        // Spawn for each spawn point in spawnPoints
        foreach (GameObject spawnPoint in spawnPoints)
            StartCoroutine(SpawnMarbleInterval(spawnPoint.GetComponent<MarbleSpawner>(), 5, 0.25f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
            int marbleColour = Random.Range(0, materials.Length);
            Material marbleMaterial = materials[marbleColour];

            marbles.Add(marbleSpawner.SpawnMarble((Colours)marbleColour, marbleMaterial));
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