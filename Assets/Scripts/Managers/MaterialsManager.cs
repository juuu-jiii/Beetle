using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the assignment of materials to GameObjects in the arena based on
/// their colours.
/// </summary>
public class MaterialsManager : MonoBehaviour
{
    /// <summary>
    /// Array of all possible marble materials that can be applied this level.
    /// </summary>
    [SerializeField]
    private Material[] materials;

    /// <summary>
    /// Number of materials in the class's underlying array.
    /// </summary>
    public int MaterialCount
    {
        get { return materials.Length; }
    }

    /// <summary>
    /// Gets the corresponding material to the specified colour.
    /// </summary>
    /// <param name="colour">
    /// The colour whose corresponding material is to be obtained.
    /// </param>
    /// <returns>
    /// The corresponding material to the specified colour.
    /// </returns>
    public Material GetMaterial(Colours colour)
    {
        return materials[(int)colour];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
