using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour {
	// Control variables - movement speed and movement method
	[Range(0.01f, 1.0f)]
	public float speed = 0.65f;
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
	void Awake () {
		octile = new Octile ();
		waypoints = new List<xyLoc>();
		astar = new GridAStar(Map.width, Map.height);
		useVelocity = true;
		transform.localScale = Vector3.one * Map.scale;
	}
	void Start()
	{
		GetAgentLocation ();
	}

	void GetAgentLocation()
	{
		currentLoc = new xyLoc (-1, -1);
		for (int x = 0; x < 100; x++)
		{
			if (map.IsOccupied (currentLoc.x, currentLoc.y))
			{
				currentLoc.x = Random.Range (0, Map.width);
				currentLoc.y = Random.Range (0, Map.height);
			} else
			{
				transform.position = GetLocation (currentLoc);
				return;
			}
		}
		Debug.Log ("Failure placing agent");
	}

	Vector3 GetLocation(xyLoc l)
	{
		return new Vector3 (l.x * Map.scale, 0.5f, l.y * Map.scale);
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
				target.transform.position = new Vector3(Mathf.RoundToInt(hit.point.x/Map.scale), 0.5f, Mathf.RoundToInt(hit.point.z/Map.scale));
				goal = new xyLoc (Mathf.RoundToInt(hit.point.x/Map.scale), Mathf.RoundToInt(hit.point.z/Map.scale));

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
		if ((waypoints.Count > 0) && Vector3.Distance(GetLocation(waypoints[waypoints.Count-1]), transform.position) < 0.01)
		{
			waypoints.RemoveAt (waypoints.Count-1);
		}

		// Draw upcoming path
		if (waypoints.Count > 0)
			Debug.DrawLine (transform.position, GetLocation(waypoints[waypoints.Count-1]), Color.red);
		for (int x = 0; x < waypoints.Count-1; x++) {
			Debug.DrawLine (GetLocation(waypoints[x]), GetLocation(waypoints[x+1]), Color.red);
		}

		// Move to next waypoint
		if (waypoints.Count > 0) {
			// Set target location
			target.transform.position = GetLocation (waypoints [waypoints.Count - 1]);

			// Update our location
			if (useVelocity) {
				Vector3 diff = GetLocation (waypoints [waypoints.Count - 1]);
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
			currentLoc = new xyLoc (Mathf.RoundToInt(transform.position.x/Map.scale), Mathf.RoundToInt(transform.position.z/Map.scale));
		}
	}

	void LateUpdate()
	{
		// Move agent after map was finished in Update()
		if (Input.GetKeyDown (KeyCode.R))
			GetAgentLocation ();
	}
}
