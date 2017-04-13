using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovementScript : MonoBehaviour {
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

	// Use this for initialization
	void Start () {
		agentLocations = new List<Vector3>();
		velocity = new Vector3 ();
		localID = globalID;
		globalID++;
		GetRandomTarget ();
		nextWanderAction = 0;
		seekTargetMix = 0.5f;
		seekGroupMix = 0.0f;
		seekGroupAlignMix = 0.0f;
		wanderMix = 0.5f;
		avoidMix = 0.0f;
		behaviorSum = 1.0f;

		if (localID == 0) {
			for (int x = 0; x < 0; x++) {
				Instantiate (this);
			}
			currentRound = 0;
		}
	}

	void OnGUI() {
		behaviorSum = (wanderMix + seekTargetMix + seekGroupMix + seekGroupAlignMix+avoidMix);
		if (behaviorSum <= 0.0f) {
			avoidMix = wanderMix = seekTargetMix = seekGroupMix = seekGroupAlignMix = 0.5f;
			behaviorSum = (wanderMix + seekTargetMix + seekGroupMix + seekGroupAlignMix+avoidMix);
		}
	}

	Vector3 GetAlignTarget()
	{
		return transform.position;
	}

	Vector3 GetChaseTarget()
	{
		Vector3 result = referenceTarget.transform.position - transform.position;
		result.Normalize();
		return result;
	}

	Vector3 GetGroupTarget()
	{
		return transform.position;
	}

	Vector3 GetWanderTarget()
	{
		if(Time.time >= nextWanderAction)
		{
			nextWanderAction = Time.time + Random.Range(minActionDuration, maxActionDuration);
		}
		return transform.position;
	}

	Vector3 GetAvoidTarget()
	{
		return transform.position;
	}

	float GetSeekAngleChange(Vector3 seekTarget)
	{
		return Mathf.Atan2(seekTarget.z, seekTarget.x) * Mathf.Rad2Deg * maxAgentTurn * Time.deltaTime;
	}
		
	void GetRandomTarget()
	{
		referenceTarget.transform.position = new Vector3 (Random.Range (0, 20)-10.0f, 0.5f, Random.Range (0, 20)-10.0f);
		nextWanderAction = 0;
	}		
		
	// Store locations of all flocking objects
	void Update () {
		if ((transform.position-referenceTarget.transform.position).sqrMagnitude < 0.5) {
			GetRandomTarget ();
		}

		if (currentRound > round) {
			round = currentRound;

			// Reset average velocity
			averageVelocity *= 0;
			// Reset average position
			agentLocations.Clear ();
		}
		agentLocations.Add (transform.position);
		averageVelocity += velocity;
	}

	// Update based on stored information from all objects
	void LateUpdate()
	{
		Vector3 goal;
		goal = wanderMix * GetWanderTarget () / behaviorSum;
		goal += seekTargetMix * GetChaseTarget () / behaviorSum;
		goal += seekGroupMix * GetGroupTarget() / behaviorSum;
		goal += seekGroupAlignMix * GetAlignTarget() / behaviorSum;
		goal += avoidMix * GetAvoidTarget () / behaviorSum;
		goal.Normalize ();
		float heading = GetSeekAngleChange (transform.position+goal);
		float agentSpeed = maxAgentSpeed * Time.deltaTime;

		float newX, newZ;
		newX = transform.position.x + Mathf.Cos(heading) * agentSpeed;
		newZ = transform.position.z + Mathf.Sin(heading) * agentSpeed;
		// Write update for agent location

		//transform.Rotate(new Vector3(0,heading,0));


		if (newX > 25)
			newX -= 50;
		else if (newX < -25)
			newX += 50;
		if (newZ > 25)
			newZ -= 50;
		else if (newZ < -25)
			newZ += 50;
		transform.position = new Vector3 (newX, 0.0f, newZ);

		currentRound++;
	}

}
