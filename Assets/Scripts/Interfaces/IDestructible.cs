using UnityEngine;

/// <summary>
/// Describes possible colours destructible objects could have.
/// </summary>
public enum Colours
{
    Red,
    Jaune,
    Green,
    Blue
}

/// <summary>
/// Describes all GameObjects that are destructible in the arena.
/// </summary>
public interface IDestructible
{
    /// <summary>
    /// The colour of this destructible object.
    /// </summary>
    Colours Colour { get; }

    /// <summary>
    /// Tracks whether this destructible object has been matched by an incoming
    /// projectile.
    /// </summary>
    bool Matched { get; set; }

    /// <summary>
    /// Sets the Colour property and material of this destructible object.
    /// </summary>
    /// <param name="colour"></param>
    /// <param name="material"></param>
    void SetColourAndMaterial(Colours colour, Material material);
}