using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MarbleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject marbleTemplate;
    public int BufferedMarbles { get; private set; }

    /// <summary>
    /// Initialises BufferedMarbles to a specified value.
    /// </summary>
    /// <param name="waveMarbleCount">
    /// Number of marbles to be spawned this wave.
    /// </param>
    public void InitBuffer(int waveMarbleCount)
    {
        BufferedMarbles = waveMarbleCount;
    }

    /// <summary>
    /// Instantiate a marble at the position and orientation of this spawner.
    /// </summary>
    /// <param name="marbleColour">
    /// The colour to be assigned to this marble.
    /// </param>
    /// <param name="marbleMaterial">
    /// The material to be applied to this marble.
    /// </param>
    /// <returns>
    /// A reference to the marble that is instantiated.
    /// </returns>
    public GameObject Spawn(Colours marbleColour, Material marbleMaterial)
    {
        GameObject marble = Instantiate(
            marbleTemplate,
            transform.position,
            transform.rotation);
        BufferedMarbles--;

        Marble marbleScript = marble.GetComponent<Marble>();

        // Set spawned marble's initial velocity and colour (via its material).
        marbleScript.Rb.velocity = marble.transform.forward * marbleScript.speed;
        marbleScript.Colour = marbleColour;
        marbleScript.GetComponent<MeshRenderer>().material = marbleMaterial;

        return marble;
    }
}
