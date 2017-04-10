using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    // Gives access to map for pathfinding purposes
    public Map map;

    // Current and goal location
    xyLoc startLoc;
    xyLoc middleLoc;
    xyLoc endLoc;

    // Use this for initialization
    void Start()
    {
        startLoc = new xyLoc(0, 0);
        middleLoc = new xyLoc(0, 0);
        endLoc = new xyLoc(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Left mouse is down in this frame
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                startLoc = new xyLoc(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                endLoc = startLoc;
            }
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                endLoc = new xyLoc(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            }
        }

        bool legal = false;
        if (LineOfSight(startLoc, endLoc))
            legal = true;
        Debug.DrawLine(new Vector3(startLoc.x, 0.5f, startLoc.y),
            new Vector3(endLoc.x, 0.5f, endLoc.y),
            legal ? Color.blue : Color.red);

    }

    // Assume we are in quadrant 0
    bool LineOfSight(xyLoc p1, xyLoc p2)
    {
        Vector2 slope = new Vector2((p2.x - p1.x), (p2.y - p1.y));
        Vector2 currentSpot = new Vector2(p1.x, p1.y);
        for (int i = p1.x; i < p2.x; i++)
        {
            currentSpot += slope / Mathf.Abs(p1.x - p2.x);
            if (map.IsOccupied((int)currentSpot.x, (int)currentSpot.y) ||
               map.IsOccupied((int)currentSpot.x, (int)currentSpot.y + 1) ||
               map.IsOccupied((int)currentSpot.x, (int)currentSpot.y - 1))
                return false;
        }
        return true;
    }
}
