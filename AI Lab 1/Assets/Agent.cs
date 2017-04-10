using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
    public GameObject target;
    [Range(0, 1)]
    public float stoppedSpeed;
    [Range(0, 1)]
    public float friction;
    [Range(0, 100)]
    public float maxVelocity;
    [Range(0,5)]
    public float speed;

     Vector3 moveVec;
	// Use this for initialization
	void Start () {
        MoveTarget();
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(transform.position, target.transform.position) < stoppedSpeed)
            MoveTarget();
	}

    void MoveTarget()
    {
        Vector2 loc = Random.insideUnitCircle * 25;
        target.transform.position = new Vector3(loc.x, target.transform.position.y, loc.y);
    }

    void LateUpdate()
    {
        Vector3 diff = target.transform.position - transform.position;
        moveVec *= friction;
        moveVec += diff * speed;
        if(moveVec.sqrMagnitude > maxVelocity * maxVelocity)
        {
            moveVec = moveVec.normalized * maxVelocity;
        }
        transform.position += moveVec;
    }
}
