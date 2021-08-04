using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MarbleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject marbleTemplate;
    private Queue<Colours> bufferedColours;

    // Getter properties - encapsulation!
    public int BufferedColours
    {
        get { return bufferedColours.Count; }
    }
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
    public GameObject Spawn(Material marbleMaterial)
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
        marbleScript.GetComponent<MeshRenderer>().material = marbleMaterial;

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
