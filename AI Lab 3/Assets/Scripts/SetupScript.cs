using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupScript : MonoBehaviour {

	public GameObject a1, t1, a2, t2;

	void StartObjects()
	{
		RVO a1s = a1.GetComponent<RVO>();
		if (a1s) {
			a1s.Go ();
		}
		else {
			Debug.Log ("Can't find script for agent 1");
		}
		RVO a2s = a2.GetComponent<RVO>();
		if (a2s) {
			a2s.Go ();
		}
		else {
			Debug.Log ("Can't find script for agent 2");
		}
	}

	bool tracking = false;
	GameObject trackedObject;
	// Update is called once per frame
	void Update () {
		// Left mouse is down in this frame
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100)) {
				if (hit.point.x < -9)
					trackedObject = t2;
				else if (hit.point.x > 9)
					trackedObject = t1;
				else if (hit.point.x < -5)
					trackedObject = a1;
				else if (hit.point.x > 5)
					trackedObject = a2;
				else {
					StartObjects ();
					return;
				}
				Vector3 tmp = trackedObject.transform.position;
				tmp.z = hit.point.z;
				trackedObject.transform.position = tmp;
				tracking = true;
			}
		}
		else if (tracking == true)
		{
			if (Input.GetMouseButton (0)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 100)) {
					Vector3 tmp = trackedObject.transform.position;
					tmp.z = hit.point.z;
					if (tmp.z < 0.25 && tmp.z > -0.25)
						tmp.z = 0;
						
					trackedObject.transform.position = tmp;
				}
			} else {
				tracking = false;
			}
		}

	}
}
