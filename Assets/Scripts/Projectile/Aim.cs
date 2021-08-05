using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the behaviour of the player's Aim Guard LineRenderer.
/// </summary>
public class Aim : MonoBehaviour
{
    public LineRenderer AimGuard { get; private set; }
    
    /// <summary>
    /// Vector3 describing the direction of the second point of the Aim Guard,
    /// relative to the transform.position of the Projectile Spawn Point
    /// GameObject this script is attached to.
    /// </summary>
    public Vector3 AimDirection { get; private set; }

    /// <summary>
    /// The location of the cursor, in screen coordinates.
    /// </summary>
    private Vector3 mousePosScreen;

    /// <summary>
    /// The location of the cursor, in world coordinates.
    /// </summary>
    private Vector3 mousePosWorld;

    /// <summary>
    /// The direction of the second point of the Aim Guard.
    /// </summary>
    private Vector3 direction;

    /// <summary>
    /// The angle between the cursor and the transform.position associated with
    /// the Projectile Spawn Point GameObject this script is attached to.
    /// </summary>
    private float theta;

    /// <summary>
    /// The length of the Aim Guard.
    /// </summary>
    [SerializeField]
    private float length;

    /// <summary>
    /// The maximum (one-sided) angle at which the player can aim, in degrees.
    /// To obtain the full range, double this value.
    /// </summary>
    [SerializeField]
    private float maxAngle;
    
    // Start is called before the first frame update
    void Start()
    {
        AimGuard = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Input.mousePosition's z-value is always 0. Set it to reflect the
        // Camera's height from the ground for accurate tracking of mouse
        // click positions.
        mousePosScreen = Input.mousePosition;
        mousePosScreen.z = Camera.main.transform.position.y;

        // Input.mousePosition returns data in terms of screen coordinates.
        // Convert this to world coordinates.
        mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);

        // The direction of the second LineRenderer point is the difference
        // between the positions of target and spawnPoint.
        direction = mousePosWorld - transform.position;

        // Calculate the angle of the mouse relative to transform.position.
        // Note 0deg = RHS, and 90deg = straight ahead. Why?
        theta = Mathf.Atan2(
            (transform.position + direction * length).z - transform.position.z,
            (transform.position + direction * length).x - transform.position.x)
            * Mathf.Rad2Deg;

        // Limit the angles at which the player can aim using trig.
        if (theta >= 90 - maxAngle && theta <= 90 + maxAngle)
        {
            // The position of the second point of the line is obtained using
            // transform.position + direction * maxDistance
            //
            // The issue was that the y-coordinate of said point was fluctuating.
            // The correct height above the ground should be 0.5; that is, the
            // value of transform.position.y. Since that is constant, the only
            // other thing that could be affecting it is direction's y-coordinate.
            // To fix that, zero direction's y-coordinate out BEFORE normalising
            // so the height is unaffected by the calculation.
            direction.y = 0;
            direction = direction.normalized;
        }
        // Snap to max angle L/R depending on whether theta is greater or less
        // than -90deg.
        else if (theta < 90 - maxAngle && theta > -90)
        {
            // Use 90deg - maxAngle, since theta is measured relative to x-axis.
            direction = new Vector3(
                length * Mathf.Cos((90 - maxAngle) * Mathf.Deg2Rad),
                transform.position.y,
                length * Mathf.Sin((90 - maxAngle) * Mathf.Deg2Rad)).normalized;
        }
        else
        {
            // Use 90deg - maxAngle, since theta is measured relative to x-axis.
            direction = new Vector3(
                -length * Mathf.Cos((90 - maxAngle) * Mathf.Deg2Rad),
                transform.position.y,
                length * Mathf.Sin((90 - maxAngle) * Mathf.Deg2Rad)).normalized;
        }

        // Update the positions of the points on the Aim Guard.
        AimGuard.SetPosition(0, transform.position);
        AimGuard.SetPosition(1, transform.position + direction * length);

        // The direction to move in is the difference between the positions
        // of target and spawnPoint.
        //
        // The result is normalised so that it can later be multiplied
        // accordingly by a speed value in ProjectileSpawner.
        AimDirection = (AimGuard.GetPosition(1) - transform.position).normalized;
    }
}
