using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManController : MonoBehaviour, ControlledObject {
	GameController.direction currentDirection;
	GameController.direction lastDirection;


	void Start () {
		currentDirection = GameController.direction.kNone;
		lastDirection = GameController.direction.kNone;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.W))
		{
			currentDirection = GameController.direction.kUp;		
		}
		else if (Input.GetKeyDown (KeyCode.S))
		{
			currentDirection = GameController.direction.kDown;		
		}
		else if (Input.GetKeyDown (KeyCode.A))
		{
			currentDirection = GameController.direction.kLeft;		
		}
		else if (Input.GetKeyDown (KeyCode.D))
		{
			currentDirection = GameController.direction.kRight;		
		}
	}

	public GameController.direction GetAction (GameMaze m, GameController c, xyLoc yourLoc)
	{
		if (c.CanMove (yourLoc, currentDirection))
		{
			lastDirection = currentDirection;
			return currentDirection;
		}
		return lastDirection;
	}

	public bool IsAlive()
	{
		return true;
	}

	public void Kill()
	{}

	public void Reset()
	{}

	public void StartPowerup()
	{
	}

	public void EndPowerup()
	{
	}
	public float GetSpeed()
	{
		return 1.0f;
	}
}
