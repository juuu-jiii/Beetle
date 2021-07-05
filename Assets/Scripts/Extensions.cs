using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
}
