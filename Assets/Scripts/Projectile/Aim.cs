using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    private LineRenderer aim;
    private Vector3 mousePos;
    private Vector3 target;
    private Vector3 direction;
    private float theta;
    [SerializeField]
    private float maxDistance;
    /// <summary>
    /// The maximum (one-sided) angle at which the player can aim, in degrees.
    /// To obtain the full range, double this value.
    /// </summary>
    [SerializeField]
    private float maxAngle;
    public Vector3 AimDirection { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        aim = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Input.mousePosition's z-value is always 0. Set it to reflect the
        // Camera's height from the ground for accurate tracking of mouse
        // click positions.
        mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;

        // Input.mousePosition returns data in terms of screen coordinates.
        // Convert this to world coordinates.
        target = Camera.main.ScreenToWorldPoint(mousePos);

        // The direction of the second LineRenderer point is the difference
        // between the positions of target and spawnPoint.
        direction = target - transform.position;

        // Calculate the angle of the mouse relative to transform.position.
        // Note 0deg = RHS, and 90deg = straight ahead. Why?
        theta = Mathf.Atan2(
            (transform.position + direction * maxDistance).z - transform.position.z,
            (transform.position + direction * maxDistance).x - transform.position.x)
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
                maxDistance * Mathf.Cos((90 - maxAngle) * Mathf.Deg2Rad),
                transform.position.y,
                maxDistance * Mathf.Sin((90 - maxAngle) * Mathf.Deg2Rad)).normalized;
        }
        else
        {
            // Use 90deg - maxAngle, since theta is measured relative to x-axis.
            direction = new Vector3(
                -maxDistance * Mathf.Cos((90 - maxAngle) * Mathf.Deg2Rad),
                transform.position.y,
                maxDistance * Mathf.Sin((90 - maxAngle) * Mathf.Deg2Rad)).normalized;
        }

        aim.SetPosition(0, transform.position);
        aim.SetPosition(1, transform.position + direction * maxDistance);

        // The direction to move in is the difference between the positions
        // of target and spawnPoint.
        //
        // The result is normalised so that it can later be multiplied
        // accordingly by a speed value in ProjectileSpawner.
        AimDirection = (aim.GetPosition(1) - transform.position).normalized;
    }
}
