using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystem : MonoBehaviour {

	string result;

	struct turtle {
		public Vector3 location;
		public float heading;
	}

	const float LAngle = 30;
	IDictionary<char, string> rules;
	// Use this for initialization
	void Start () {
		rules = new Dictionary<char, string> ();

        result = "F";
        rules['F'] = "F[-F]F[+F][F]";

        //		result = "f";
        //		rules ['F'] = "FF";
        //		rules ['f'] = "F−[[f]+f]+F[+Ff]−f";

        //result = "F";
        //rules ['F'] = "FF−[−F+F+F]+[+F−F−F]";
    }

	void RewriteRule(ref string s)
	{
        // 1. Make new result string
        // 2. For each character c in s
        //    If c matches in dictionary, append replacement to result
        //    else append c to result
        // Put result in s
        string res = "";
        foreach(char character in s)
        {
            if (rules.ContainsKey(character))
            {
                res += rules[character];
            }
            else res += character;
        }
        Debug.Log("String result: " + res);
        s = res;
	}

	void MouseControl()
	{
		float h = Input.GetAxis("Mouse X");
		float v = Input.GetAxis("Mouse Y");
		Camera.main.transform.Rotate(-v, h, 0);
		if (Input.GetKey(KeyCode.S))
		{
			Camera.main.transform.position -= Camera.main.transform.forward;
		}
		if (Input.GetKey(KeyCode.W))
		{
			Camera.main.transform.position += Camera.main.transform.forward;
		}
		if (Input.GetKey(KeyCode.D))
		{
			Camera.main.transform.position += Camera.main.transform.right;
		}
		if (Input.GetKey(KeyCode.A))
		{
			Camera.main.transform.position -= Camera.main.transform.right;
		}
	}

	// Update is called once per frame
	void Update () {
		MouseControl ();
		if (Input.GetKeyDown(KeyCode.R))
		{
			RewriteRule (ref result);
		}
		if (Input.GetKeyDown(KeyCode.Backslash))
		{
			result = "F";
		}
		// render at 0,0
		Render (result, new turtle ());
	}

	void Render(string s, turtle state)
	{
		Stack<turtle> stack = new Stack<turtle> ();
		int loc = 0;
        while (loc < s.Length)
		{
			switch (s[loc])
			{
			case '[':
				{
                        // Code here:
                        stack.Push(state);
					break;
				}
			case ']':
				{
                        // Code here:
                        turtle t = stack.Pop();
                        state.location = t.location;
                        state.heading = t.heading;
					break;
				}
			case 'F': 
				{
                        // Code here:
                        Vector3 result = (Quaternion.Euler(state.heading,0, 0) * Vector3.up).normalized;
                        state.location += result;
                        Debug.DrawLine(state.location - result, state.location,Color.red);
					break;
				}
				break;
			case 'f': 
				{
                        // Code here:
                        state.location += (Quaternion.Euler(state.heading, 0, 0) * Vector3.up).normalized;
					break;
				}
				break;
			case '+':
				{
                        // Code here:
                        state.heading += LAngle;
					break;
				}
			case '-':
				{
                        // Code here:
                        state.heading -= LAngle;
					break;
				}
			}
			loc++;
		}
	}
}
