using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes the properties/behaviours of targets in the arena.
/// </summary>
public class Target : MonoBehaviour, IDestructible
{
    [SerializeField]
    private GameObject materialsManager;
    private MaterialsManager materialsManagerScript;

    public Colours Colour { get; private set; }
    public bool Matched { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        materialsManagerScript = materialsManager.GetComponent<MaterialsManager>();

        // Randomly assign a colour on game start and set this target's
        // material accordingly.
        Colours colour = materialsManagerScript.GetRandomColour();
        SetColourAndMaterial(colour, materialsManagerScript.GetMaterial(colour));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColourAndMaterial(Colours colour, Material material)
    {
        Colour = colour;
        GetComponent<MeshRenderer>().material = material;
    }
}
