using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MarbleSpawner : MonoBehaviour
{
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

    //private void SetColour(Material[] materials, Marble marbleScript)
    //{
    //    int marbleColourIndex = Random.Range(0, materials.Length);
    //    marbleScript.Colour = (Colours)marbleColourIndex;
    //    marbleScript.GetComponent<MeshRenderer>().material = materials[marbleColourIndex];
    //}

    /// <summary>
    /// Instantiate a marble at the position and orientation of this spawner.
    /// </summary>
    /// <param name="marbleColour">
    /// The colour to be assigned to this marble.
    /// </param>
    /// <param name="materialColour">
    /// The material to be applied to this marble.
    /// </param>
    /// <returns>
    /// A reference to the marble that is instantiated.
    /// </returns>
    public GameObject Spawn(Colours colour, Color materialColour)
    {
        //Debug.Log(string.Format("position: {0} rotation: {1}", spawner.transform.position, spawner.transform.rotation));
        GameObject marble = Instantiate(
            marbleTemplate,
            transform.position,
            transform.rotation);
        BufferedMarbles--;

        Marble marbleScript = marble.GetComponent<Marble>();

        // Set spawned marble's initial velocity and colour (via its material).
        marbleScript.Rb.velocity = marble.transform.forward * marbleScript.speed;
        marbleScript.Colour = colour;
        marbleScript.GetComponent<MeshRenderer>().material.color = materialColour;

        return marble;
    }
}
