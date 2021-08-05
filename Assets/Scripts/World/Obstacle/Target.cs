using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes the properties/behaviours of targets in the arena.
/// </summary>
public class Target : MonoBehaviour
{
    [SerializeField]
    private GameObject materialsManager;
    private MaterialsManager materialsManagerScript;

    [SerializeField]
    private Colours colour;

    // Start is called before the first frame update
    void Start()
    {
        materialsManagerScript = materialsManager.GetComponent<MaterialsManager>();

        // Randomly assign a colour on game start and set this target's
        // material accordingly.
        colour = materialsManagerScript.GetRandomColour();
        GetComponent<MeshRenderer>().material = materialsManagerScript.GetMaterial(colour);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
