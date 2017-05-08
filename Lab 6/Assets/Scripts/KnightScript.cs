using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightScript : MonoBehaviour
{
	// Map is 16 x 10
	float cutoffX, cutoffY;
	float xDir, yDir;
	MoveType move;

	enum MoveType
	{
		MoveUp,
		MoveDown,
		MoveLeft,
		MoveRight

	}
	// Use this for initialization
	void Start ()
	{
		cutoffX = Random.Range (6.0f, 8.0f);
		cutoffY = Random.Range (-5.0f, -2.0f);

		xDir = Vector3.right.x;
		yDir = Vector3.down.y;

		Debug.Log ("CutoffX: " + cutoffX + " CutoffY: " + cutoffY);
	}
	// Update is called once per frame
	void Update ()
	{
		CheckBoundaries ();
	}

	void ChooseCutoff (bool xDirection)
	{
		if (xDirection) {
			cutoffX *= -1;
		} else
			cutoffY *= -1; 
	}

	void Movement (MoveType mT)
	{
		switch (mT) {
		case MoveType.MoveDown:
			transform.position += Vector3.down * Time.deltaTime * 3;
			break;
		case MoveType.MoveLeft:
			transform.position += Vector3.left * Time.deltaTime * 3;
			break;
		case MoveType.MoveRight:
			transform.position += Vector3.right * Time.deltaTime * 3;
			break;
		case MoveType.MoveUp:
			transform.position += Vector3.up * Time.deltaTime * 3;
			break;
		default:
			break;
		}
	}

	void CheckBoundaries ()
	{
		float randVal = Random.Range (0.0f, 1.0f);
		switch (move) {
		case MoveType.MoveDown:
			if (transform.position.y < cutoffY) {
				if (randVal < .5f)
					move = MoveType.MoveUp;
				else if (transform.position.x > 0) {
					move = MoveType.MoveLeft;
				} else
					move = MoveType.MoveRight;

				ChooseCutoff (false);
			}
			break;
		case MoveType.MoveUp:
			if (transform.position.y > cutoffY) {
				if (randVal < .5f)
					move = MoveType.MoveDown;
				else if (transform.position.x > 0) {
					move = MoveType.MoveLeft;
				} else
					move = MoveType.MoveRight;
				ChooseCutoff (false);
			}
			break;
		case MoveType.MoveRight:
			if (transform.position.x > cutoffX) {
				if (randVal < .5f)
					move = MoveType.MoveLeft;
				else if (transform.position.y > 0)
					move = MoveType.MoveDown;
				else
					move = MoveType.MoveUp;
				
				ChooseCutoff (true);
			}
			break;
		case MoveType.MoveLeft:
			if (transform.position.x < cutoffX) {
				if (randVal < .5f)
					move = MoveType.MoveRight;
				else if (transform.position.y > 0)
					move = MoveType.MoveDown;
				else
					move = MoveType.MoveUp;
			}
			break;
		default:
			break;
				
		}

		Movement (move);

	}
}
