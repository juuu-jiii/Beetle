using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> marbles;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject[] spawnPoints;
    [SerializeField]
    private Material[] materials;
    [SerializeField]
    private int waves;
    [SerializeField]
    private int marbleWaveCount;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening(Events.MarbleMatch, HandleCollision);


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
            // increment score -- maybe move this into another class so scoring
            // and combos can be handled within.
            //
            // Call that method using the event system.
        }
    }
}
