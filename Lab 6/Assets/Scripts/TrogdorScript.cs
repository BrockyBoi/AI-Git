using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrogdorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.RightArrow) == true)
			transform.position += new Vector3 (1.0f, 0.0f)*Time.deltaTime;
		else if (Input.GetKey (KeyCode.LeftArrow) == true)
			transform.position += new Vector3 (-1.0f, 0.0f)*Time.deltaTime;

		if (Input.GetKey (KeyCode.UpArrow) == true)
			transform.position += new Vector3 (0.0f, +1.0f)*Time.deltaTime;
		else if (Input.GetKey (KeyCode.DownArrow) == true)
			transform.position += new Vector3 (0.0f, -1.0f)*Time.deltaTime;
	}
}
