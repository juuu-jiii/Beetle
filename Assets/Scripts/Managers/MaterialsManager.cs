using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the assignment of materials to GameObjects in the arena based on
/// their colours.
/// </summary>
public class MaterialsManager : MonoBehaviour
{
    #region Unserialisable Dictionary Workaround
    /// <summary>
    /// Array containing colours that will spawn this level.
    /// </summary>
    [SerializeField]
    private Colours[] allowedColours;

    /// <summary>
    /// Array of all possible marble materials that can be applied this level.
    /// </summary>
    [SerializeField]
    private Material[] allowedMaterials;
    #endregion

    /// <summary>
    /// Maps colours to their corresponding materials.
    /// </summary>
    private Dictionary<Colours, Material> colourMaterialMapper;

    /// <summary>
    /// Number of entries in the class's underlying dictionary.
    /// </summary>
    public int EntryCount
    {
        get { return colourMaterialMapper.Count; }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        colourMaterialMapper = new Dictionary<Colours, Material>();
        
        // Populate colourMaterialMapper. Do not use ContainsKey(), since it is
        // an error if repeated entries exist.
        for (int i = 0; i < allowedColours.Length; i++)
        {
            colourMaterialMapper.Add(
                allowedColours[i],
                allowedMaterials[i]);
        }
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
        return colourMaterialMapper[colour];
    }

    /// <summary>
    /// Gets a random colour from the allowedColours array.
    /// </summary>
    /// <returns>
    /// The result of the random colour generation.
    /// </returns>
    public Colours GetRandomColour()
    {
        return allowedColours[Random.Range(0, allowedColours.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
