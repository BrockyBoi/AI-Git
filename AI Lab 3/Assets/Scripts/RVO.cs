using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RVO : MonoBehaviour
{
    public GameObject target;
    public GameObject otherAgent;
    [Range(0.5F, 5.0F)]
    public float speed;
    public bool showDebug;
    // Are we moving across world
    bool running = false;
    float timeToReset;

    Vector3 lastDirection;
    Vector3 direction;
    Vector3 cachedStartLoc;
    List<Vector3> path;

    void Start()
    {
        speed = 1.5f;
        path = new List<Vector3>();
        showDebug = false;
    }

    public void Go()
    {
        if (running || timeToReset > 0)
        {
            transform.position = cachedStartLoc;
        }
        // start agent moving
        running = true;
        lastDirection = target.transform.position - transform.position;
        lastDirection.Normalize();
        lastDirection *= speed;
        cachedStartLoc = transform.position;
        direction = lastDirection;
        path.Clear();
        path.Add(transform.position);
        timeToReset = 3f;
    }

    public void Reset()
    {
        path.Clear();
        timeToReset = 0;
        running = false;
        transform.position = cachedStartLoc;
    }

    /*
	 * On = 0; Left > 0; Right < 0
	 */
    float GetSide(Vector3 line1, Vector3 line2, Vector3 point)
    {
        return ((line2.x - line1.x) * (point.z - line1.z) - (line2.z - line1.z) * (point.x - line1.x));
    }

    // Update is called once per frame
    void Update()
    {
        // Reset to initial condition
        if ((transform.position - target.transform.position).sqrMagnitude < 0.05)
        {
            running = false;
            timeToReset -= Time.deltaTime;
            if (timeToReset < 0)
            {
                RVO other = otherAgent.GetComponent<RVO>();
                other.Reset();
                Reset();
            }
        }

        if (running)
        {
            direction = target.transform.position - transform.position;
            direction.Normalize();
            direction *= speed;

            RVO other = otherAgent.GetComponent<RVO>();
            // Get our heading after subtracting opposite agent heading
            Vector3 adjustedHeading = direction - other.lastDirection;
            // Get our speed in that heading
            float adjustedSpeed = adjustedHeading.magnitude;
            // Get heading towards the other agent
            Vector3 headingToAgent = other.transform.position - transform.position;

            // Get ratio so we can compute the angle to go around the other agent
            float ratio = 2.0f / headingToAgent.magnitude;
            if (ratio > 1)
            {
                Debug.Log("Collision!");
                ratio = 1;
            }
            // Compute angle to go around agent
            float angle = Mathf.Asin(ratio) * Mathf.Rad2Deg;

            // Get vectors for heading right/left
            Vector3 rightHeading = Quaternion.Euler(0, angle, 0)  * headingToAgent;
            Vector3 leftHeading = Quaternion.Euler(0, -angle, 0) * headingToAgent;
            // Normalize vectors
            rightHeading = rightHeading.normalized * adjustedSpeed;
            leftHeading = leftHeading.normalized * adjustedSpeed;

            // Draw debug information
            if (showDebug)
            {
                Debug.DrawLine(transform.position, transform.position + rightHeading, Color.red);
                Debug.DrawLine(transform.position, transform.position + headingToAgent, Color.white);
                Debug.DrawLine(transform.position, transform.position + leftHeading, Color.yellow);
                Debug.DrawLine(transform.position, transform.position + direction * 2, Color.grey);
                Debug.DrawLine(transform.position, transform.position + adjustedHeading * 2, Color.blue);
            }

            // Check if we have a VO collision
            if (GetSide(Vector3.zero, leftHeading, adjustedHeading) < 0 &&
                GetSide(Vector3.zero, rightHeading,  adjustedHeading) > 0)
            {
                //				Debug.Log ("On target to collide");

                // Move towards the side that is closest to our current velocity
                if (Vector3.Distance(adjustedHeading, rightHeading) < Vector3.Distance(adjustedHeading, leftHeading))
                {
                    //					Debug.Log("Going right");
                    direction = (rightHeading + other.lastDirection);
                    //					direction = (rightHeading + other.lastDirection) * 0.5f + 0.5f * direction;
                }
                else
                {
                    //					Debug.Log("Going left");
                    direction = (leftHeading + other.lastDirection);
                    //					direction = (leftHeading+other.lastDirection) * 0.5f + 0.5f * direction;
                }
                if (showDebug)
                    Debug.DrawLine(transform.position, transform.position + direction * 2, Color.magenta);

            }
        }
    }

    void LateUpdate()
    {
        if (running)
        {
            transform.position += direction * Time.deltaTime;
            path.Add(transform.position);
            lastDirection = direction;
        }

        for (int x = 0; x < path.Count - 1; x++)
        {
            Debug.DrawLine(path[x], path[x + 1], Color.black);
        }
    }
}
