using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    private LineRenderer aim;
    private Vector3 mousePos;
    private Vector3 target;
    private Vector3 direction;
    [SerializeField]
    private float maxDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        aim = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;

        target = Camera.main.ScreenToWorldPoint(mousePos);

        direction = target - transform.position;

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

        aim.SetPosition(0, transform.position);
        aim.SetPosition(1, transform.position + direction * maxDistance);

        Vector3 test = transform.position;
        test.z += maxDistance;

        //Debug.Log(transform.position + direction * maxDistance);
        //Debug.Log(string.Format("{0}, {1}", transform.position, test));
        //Debug.Log(string.Format("{0}, {1}", test, transform.position + direction * maxDistance));
        //Debug.Log(Vector3.Angle(transform.position + direction * maxDistance, test));
        Debug.Log(Mathf.Atan2(
            (transform.position + direction * maxDistance).z - test.z,
            (transform.position + direction * maxDistance).x - test.x
            ) * Mathf.Rad2Deg * 2);
        //Debug.Log(Vector3.Angle(transform.position, transform.position + direction * maxDistance));
    }
}
