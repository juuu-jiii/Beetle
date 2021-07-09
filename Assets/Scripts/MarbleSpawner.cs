using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject spawner;
    [SerializeField]
    private GameObject marbleTemplate;
    //private List<GameObject> buffer;
    public int BufferedMarbles { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Initialises BufferedMarbles to a specified value.
    /// </summary>
    /// <param name="waveMarbleCount">
    /// Number of marbles to be spawned this wave.
    /// </param>
    public void InitBuffer(int waveMarbleCount)
    {
        BufferedMarbles = waveMarbleCount;
        //for (int i = 0; i < waveMarbleCount; i++)
        //{
        //    buffer.Add(Extensions.InstantiateMarble(
        //        marbleTemplate,
        //        transform.position,
        //        transform.rotation
        //        /*false*/) as GameObject);
        //}
    }

    // SpawnMarbles()
    // Write a function that accepts a List<GameObject>. This function will
    // populate that List with marbles, instantiating them as it does. So:
    // List.Add(Extensions.InstantiateMarble()). There should be no need to
    // return the list, since it is passed by reference, and is not reassigned,
    // only modified.

    /// <summary>
    /// Instantiate a marble at the position and orientation of this spawner.
    /// </summary>
    /// <returns>
    /// A reference to the marble that was instantiated.
    /// </returns>
    public GameObject SpawnMarble()
    {
        //Debug.Log(string.Format("position: {0} rotation: {1}", spawner.transform.position, spawner.transform.rotation));
        GameObject marble = Instantiate(
            marbleTemplate,
            spawner.transform.position,
            spawner.transform.rotation);
        BufferedMarbles--;

        // Set spawned marble's initial velocity.
        MarbleMovement marbleScript = marble.GetComponent<MarbleMovement>();
        Debug.Log(marbleScript.rb);
        marbleScript.rb.velocity = marble.transform.forward * marbleScript.speed;

        return marble;
    }
}
