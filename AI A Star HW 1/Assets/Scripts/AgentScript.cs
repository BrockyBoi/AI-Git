using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour {
	// Control variables - movement speed and movement method
	[Range(0.01f, 1.0f)]
	public float speed = 0.15f;
	public bool useVelocity = true;

	// Control variable - draw search debug info
	public bool showDebug = false;

	// Controls the object used to indicate our next target
	public GameObject target;
	// Gives access to map for pathfinding purposes
	public Map map;
	// Heuristic for searching
	Octile octile;

	// Used for searching
	GridAStar astar;

	// List of waypoints to follow
	List<xyLoc> waypoints;

	// Current and goal location
	xyLoc currentLoc;
	xyLoc goal;

	// Control for managing timeslicing of search
	bool doingSearch = false;

	// Use this for initialization
	void Start () {
		octile = new Octile ();
		waypoints = new List<xyLoc>();

		// Sample set of waypoints. Movement is from last to first
		waypoints.Add (new xyLoc ( 0,  0));
		waypoints.Add (new xyLoc (49,  0));
		waypoints.Add (new xyLoc (49, 49));
		waypoints.Add (new xyLoc ( 0, 49));
		astar = new GridAStar(50, 50);
		useVelocity = true;
	}
	
	// Update is called once per frame
	void Update () {
		// 1. Timeslice search if it is ongoing. One expansion for frame
		//    In practice you would do more.
		if (doingSearch) {
			if (astar.Step (currentLoc, goal, map)) {
				doingSearch = false;
				waypoints = astar.ExtractPath ();
			}
		}
		if (showDebug)
			astar.DebugDraw ();

		// Left mouse is down in this frame
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100)) {
				target.transform.position = new Vector3(Mathf.RoundToInt(hit.point.x), 0.5f, Mathf.RoundToInt(hit.point.z));
				goal = new xyLoc (Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));

				// Initialize incremental search
				if (showDebug) {
					astar.Init (currentLoc, goal, map, octile);
					doingSearch = true;
				} else {
					// perform full search immediately
					waypoints = astar.GetPath (currentLoc, goal, map, octile);
				}
			}
		}

		// Don't do movement if we are searching
		if (doingSearch)
			return;

		// Reached current waypoint
		if ((waypoints.Count > 0) &&
			(waypoints[waypoints.Count-1].SquaredDistance (transform.position.x, transform.position.z) < 0.01)) {
			waypoints.RemoveAt (waypoints.Count-1);
		}

		// Draw upcoming path
		if (waypoints.Count > 0)
			Debug.DrawLine (transform.position, new Vector3 (waypoints [waypoints.Count-1].x, 1.0f, waypoints [waypoints.Count-1].y), Color.red);
		for (int x = 0; x < waypoints.Count-1; x++) {
			Debug.DrawLine (new Vector3 (waypoints [x].x, 1.0f, waypoints [x].y), new Vector3 (waypoints [x + 1].x, 1.0f, waypoints [x + 1].y), Color.red);
		}

		// Move to next waypoint
		if (waypoints.Count > 0) {
			// Set target location
			target.transform.position = new Vector3 (waypoints [waypoints.Count-1].x, target.transform.position.y, waypoints [waypoints.Count-1].y);

			// Update our location
			if (useVelocity) {
				Vector3 diff = new Vector3 (waypoints [waypoints.Count-1].x, transform.position.y, waypoints [waypoints.Count-1].y);
				diff -= transform.position;
				if (diff.magnitude > speed) {
					diff.Normalize ();
					diff *= speed;
				}
				transform.position += diff;
			} else {
				transform.position = new Vector3 (transform.position.x * 0.9f + waypoints [waypoints.Count-1].x * 0.1f,
					transform.position.y,
					transform.position.z * 0.9f + waypoints [waypoints.Count-1].y * 0.1f);
			}
			currentLoc = new xyLoc (Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
		}
	}
}
