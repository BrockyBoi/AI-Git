using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovementScript : MonoBehaviour
{
    // Variables for coordinating all agents
    static int globalID = 0;
    static int round = 0;

    // Unique id for this agent [0...globalID)
    int localID;
    // Locally what round we think we are in
    int currentRound;

    // Shared by all agents
    public GameObject referenceTarget;

    // Control dials
    [Range(0.0F, 1.0F)]
    public float wanderMix = 0.0F;
    [Range(0.0F, 1.0F)]
    public float seekTargetMix = 1.0F;
    [Range(0.0F, 1.0F)]
    public float seekGroupMix = 1.0F;
    [Range(0.0F, 1.0F)]
    public float seekGroupAlignMix = 1.0F;
    [Range(0.0F, 1.0F)]
    public float avoidMix = 0.5F;

    float behaviorSum;

    [Range(0, 660)]
    public int maxAgentTurn = 120; // degrees per second
    [Range(0, 10)]
    public float maxAgentSpeed = 3.5f; // units per second

    const float minActionDuration = 1.0f;
    const float maxActionDuration = 3.0f;

    // Wandering variables
    float nextWanderAction;
    Vector3 wanderOffset;

    // Our current velocity
    Vector3 velocity;

    // Global agent information
    static Vector3 averageVelocity;
    static List<Vector3> agentLocations;

    static Vector3 centerLoc;

    // Use this for initialization
    void Start()
    {
        agentLocations = new List<Vector3>();
        velocity = new Vector3();
        localID = globalID;
        globalID++;
        GetRandomTarget();
        nextWanderAction = 0;
        seekTargetMix = 0.5f;
        seekGroupMix = 0.0f;
        seekGroupAlignMix = 0.0f;
        wanderMix = 0.5f;
        avoidMix = 0.0f;
        behaviorSum = 1.0f;

        if (localID == 0)
        {
            for (int x = 0; x < 25; x++)
            {
                Instantiate(this);
            }
            currentRound = 0;
        }
    }

    void OnGUI()
    {
        behaviorSum = (wanderMix + seekTargetMix + seekGroupMix + seekGroupAlignMix + avoidMix);
        if (behaviorSum <= 0.0f)
        {
            avoidMix = wanderMix = seekTargetMix = seekGroupMix = seekGroupAlignMix = 0.5f;
            behaviorSum = (wanderMix + seekTargetMix + seekGroupMix + seekGroupAlignMix + avoidMix);
        }
    }

    Vector3 GetAlignTarget()
    {
        // Vector3 totalHeading = Vector3.zero;
        // foreach (Vector3 heading in agentLocations)
        // {
        //     totalHeading += heading;
        // }

        averageVelocity /= agentLocations.Count;

        return averageVelocity.normalized;
    }

    Vector3 GetChaseTarget()
    {
        Vector3 result = referenceTarget.transform.position - transform.position;
        result.Normalize();
        return result;
    }

    Vector3 GetGroupTarget()
    {
        if (localID == 1)
        {
            Vector3 totalVec = Vector3.zero;
            foreach (Vector3 position in agentLocations)
            {
                totalVec += position;
            }
            totalVec /= agentLocations.Count;

            // Debug.DrawLine(transform.position, totalVec, Color.red);
            // Debug.DrawLine(transform.position, new Vector3(0,0,0), Color.red);
            // Debug.DrawLine(transform.position, new Vector3(-25,0,-25), Color.red);

            centerLoc = totalVec;
        }

        return (centerLoc - transform.position).normalized;
    }

    Vector3 GetWanderTarget()
    {
        if (Time.time >= nextWanderAction)
        {
            nextWanderAction = Time.time + Random.Range(minActionDuration, maxActionDuration);
            wanderOffset = new Vector3(Random.Range(-2.5f, 2.5f), 0, Random.Range(-2.5f, 2.5f));
        }
        wanderOffset.Normalize();

        return wanderOffset;
    }

    Vector3 GetAvoidTarget()
    {
        Vector3 pos = transform.position;
        Vector3 min1 = Vector3.one * 1000;
        Vector3 min2 = Vector3.one * 1000;

        int xOffset;
        int zOffset;

        if (transform.position.x < 0f)
            xOffset = 50;
        else xOffset = -50;

        if (transform.position.z < 0f)
            zOffset = 50;
        else zOffset = -50;

        foreach (Vector3 position in agentLocations)
        {
            if (position == pos)
                continue;

            Vector3 offSet = Vector3.zero;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        offSet = new Vector3(0, 0, 0);
                        break;
                    case 1:
                        offSet = new Vector3(xOffset, 0, 0);
                        break;
                    case 2:
                        offSet = new Vector3(0, 0, zOffset);
                        break;
                    case 3:
                        offSet = new Vector3(xOffset, 0, zOffset);
                        break;
                    default:
                        break;
                }

                if (Vector3.Distance(pos + offSet, position) < Vector3.Distance(pos, min1))
                {
                    min1 = position;
                }
            }
        }

        foreach (Vector3 position in agentLocations)
        {
            if (pos == position || position == min1)
                continue;

            Vector3 offSet = Vector3.zero;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        offSet = new Vector3(0, 0, 0);
                        break;
                    case 1:
                        offSet = new Vector3(xOffset, 0, 0);
                        break;
                    case 2:
                        offSet = new Vector3(0, 0, zOffset);
                        break;
                    case 3:
                        offSet = new Vector3(xOffset, 0, zOffset);
                        break;
                    default:
                        break;
                }
                if (Vector3.Distance(pos + offSet, position) < Vector3.Distance(pos, min2))
                {
                    min2 = position;
                }
            }
        }


        Vector3 avoidVec = transform.position - ((min1 + min2) / 2);
        avoidVec.Normalize();

        if (localID == 1)
            Debug.DrawLine(transform.position, avoidVec, Color.red);
        return avoidVec;
    }

    float GetSeekAngleChange(Vector3 seekTarget)
    {
        float a = Mathf.Atan2(seekTarget.x, seekTarget.z) * Mathf.Rad2Deg;
        float h = transform.eulerAngles.y;

        if (h > 180)
            h -= 360;
        float diff = a - h;
        if (Mathf.Abs(diff) > 180 && diff < 0)
            diff += 360;
        else if (Mathf.Abs(diff) > 180 && diff > 0)
            diff -= 360;

        return diff;
    }

    void GetRandomTarget()
    {
        referenceTarget.transform.position = new Vector3(Random.Range(0, 20) - 10.0f, 0.5f, Random.Range(0, 20) - 10.0f);
        nextWanderAction = 0;
    }

    // Store locations of all flocking objects
    void Update()
    {
        if ((transform.position - referenceTarget.transform.position).sqrMagnitude < 0.5)
        {
            GetRandomTarget();
        }

        if (currentRound > round)
        {
            round = currentRound;

            // Reset average velocity
            averageVelocity *= 0;
            // Reset average position
            agentLocations.Clear();
        }
        agentLocations.Add(transform.position);
        averageVelocity += velocity;
    }

    // Update based on stored information from all objects
    void LateUpdate()
    {
        Vector3 goal;
        goal = wanderMix * GetWanderTarget() / behaviorSum;
        goal += seekTargetMix * GetChaseTarget() / behaviorSum;
        goal += seekGroupMix * GetGroupTarget() / behaviorSum;
        goal += seekGroupAlignMix * GetAlignTarget() / behaviorSum;
        goal += avoidMix * GetAvoidTarget() / behaviorSum;
        goal.Normalize();

        float heading = GetSeekAngleChange(goal);

        heading = Mathf.Clamp(heading, -maxAgentTurn * Time.deltaTime, maxAgentTurn * Time.deltaTime);
        //Debug.Log("Heading: " + heading);
        float agentSpeed = maxAgentSpeed * Time.deltaTime;

        float newX, newZ;
        transform.Rotate(0, heading, 0);
        //newX = transform.position.x + Mathf.Cos(heading) * agentSpeed;
       // newZ = transform.position.z + Mathf.Sin(heading) * agentSpeed;
        transform.Translate(0, 0, agentSpeed);
        newX = transform.position.x;
        newZ = transform.position.z;
       // velocity = new Vector3(newX, 0, newZ);
        //Debug.Log("NewX: " + newX + " 	NewZ: " + newZ);
        // Write update for agent location


        if (newX > 25)
            newX -= 50;
        else if (newX < -25)
            newX += 50;
        if (newZ > 25)
            newZ -= 50;
        else if (newZ < -25)
            newZ += 50;
        transform.position = new Vector3(newX, 0.0f, newZ);
        velocity = transform.forward * agentSpeed;

        currentRound++;
    }

}
