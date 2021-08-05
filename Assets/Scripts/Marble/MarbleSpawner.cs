using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handles the spawning of marbles at the assigned spawn point each wave.
/// </summary>
public class MarbleSpawner : MonoBehaviour
{
    /// <summary>
    /// Template prefab for marbles.
    /// </summary>
    [SerializeField]
    private GameObject marbleTemplate;

    [SerializeField]
    private GameObject materialsManager;
    private MaterialsManager materialsManagerScript;

    // A queue was chosen because only one colour needs to be obtained and
    // removed at a time. Calling Dequeue() is more efficient than doing
    // Remove() on a List. The Peek() method also exposes a way for
    // SpawnManager to get the corresponding material for the next marble to
    // be spawned.
    //
    // A stack could have been used as well.
    /// <summary>
    /// Queue of buffered colours for the next wave of marbles.
    /// </summary>
    private Queue<Colours> bufferedColours;

    // Getter properties - encapsulation!
    // TODO DELETE unused?
    public int BufferedColours
    {
        get { return bufferedColours.Count; }
    }

    /// <summary>
    /// The colour of the next marble to be spawned.
    /// </summary>
    public Colours Next
    {
        get { return bufferedColours.Peek(); }
    }

    // NOTE: SpawnManager would execute before MarbleSpawner, and this caused
    // NullReferenceExceptions, since it would call BufferSpawnPoints(), which
    // clears each MarbleSpawner instance's bufferedColours, before the
    // instances' Start() hooks were run. The Script Execution Order was
    // edited to fix this (Edit > Project Settings > Script Execution Order).
    // An alternative would be to place time-sensitive initialisation logic in
    // Awake() instead.
    private void Start()
    {
        bufferedColours = new Queue<Colours>();
        materialsManagerScript = materialsManager.GetComponent<MaterialsManager>();
    }

    /// <summary>
    /// Instantiate a marble at the position and orientation of this spawner.
    /// </summary>
    /// <param name="marbleMaterial">
    /// The material to be applied to this marble.
    /// </param>
    /// <returns>
    /// A reference to the marble that is instantiated.
    /// </returns>
    public GameObject Spawn()
    {
        // Get the next queued colour.
        Colours marbleColour = bufferedColours.Dequeue();
        
        GameObject marble = Instantiate(
            marbleTemplate,
            transform.position,
            transform.rotation);

        Marble marbleScript = marble.GetComponent<Marble>();

        // Set spawned marble's initial velocity and colour (via its material).
        marbleScript.Rb.velocity = marble.transform.forward * marbleScript.speed;
        marbleScript.Colour = marbleColour;
        marbleScript.GetComponent<MeshRenderer>().material = 
            materialsManagerScript.GetMaterial(marbleColour);

        return marble;
    }

    /// <summary>
    /// Adds the specified colour to to the bufferedColours queue. Called during
    /// buffering, after the queue has been cleared.
    /// </summary>
    /// <param name="colour">
    /// The colour to the enqueued.
    /// </param>
    public void BufferColour(Colours colour)
    {
        bufferedColours.Enqueue(colour);
    }

    /// <summary>
    /// Clears the bufferedColours queue. Called during buffering, before 
    /// new colours are queued.
    /// </summary>
    public void ClearBuffer()
    {
        // Handle initialisation
        //if (bufferedColours == null) bufferedColours = new Queue<Colours>();
        bufferedColours.Clear();
    }
}
