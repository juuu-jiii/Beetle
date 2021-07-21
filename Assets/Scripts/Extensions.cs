using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum RenderingModes
{
    Opaque,
    Transparent
}

/// All globally-accessible extension methods for game go here.
public static class Extensions
{
    /// <summary>
    /// Instantiates a marble, and sets its hasRandomDirection bool. An extension of Object.Instantiate().
    /// </summary>
    /// <returns>
    /// A reference to the instantiated marble, as type Object.
    /// </returns>
    //public static Object InstantiateMarble(
    //    Object original,
    //    Vector3 position,
    //    Quaternion rotation
    //    /*bool hasRandomDirection*/)
    //{
    //    GameObject marble = Object.Instantiate(original, position, rotation) as GameObject; // can also perform explicit cast
    //    //MarbleMovement marbleScript = marble.GetComponent<MarbleMovement>();
    //    //marbleScript.hasRandomDirection = hasRandomDirection;
    //    return marble;
    //}

    // refactor InstantiateMarble()
    // might need to include a bool on whether the marble is a projectile
    // Otherwise this will be the same as the original Object.Instantiate(). If
    // later on this is found to be unnecessary, just comment out the extension
    // method altogether.

    /// <summary>
    /// Changes a material's Rendering Mode by applying several changes to it.
    /// Taken from
    /// https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/StandardShaderGUI.cs
    /// </summary>
    /// <param name="material">
    /// Material whose Rendering Mode is to be changed. 
    /// </param>
    /// <param name="RenderingMode">
    /// The type of Rendering Mode to change to.
    /// </param>
    public static void ChangeRenderMode(Material material, RenderingModes RenderingMode)
    {
        switch (RenderingMode)
        {
            case RenderingModes.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingModes.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }
}
