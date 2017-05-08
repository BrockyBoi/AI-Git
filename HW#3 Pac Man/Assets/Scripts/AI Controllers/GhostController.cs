using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GhostSpecialization
{
	void GetTarget (out int x, out int y, GhostController.ghostMode currentMode, GameController c);

	int GetStartDelay ();

	GhostController.ghostMode GetStartMode ();

	Color GetBaseColor ();
}


public class GhostController : MonoBehaviour, ControlledObject
{
	GameController.direction lastDirection = GameController.direction.kLeft;
	Color baseColor;

	public enum ghostMode
	{
		kInHouse,
		kLeavingHouse,
		kScatter,
		kChase,
		kFrightened,
		kDead
	}

	ghostMode lastMode;
	ghostMode currentMode;
	float timeElapsed;

	void Start ()
	{
		timeElapsed = 0;
		currentMode = lastMode = this.GetComponent<GhostSpecialization> ().GetStartMode ();

		this.GetComponent<Renderer> ().material.shader = Shader.Find ("Transparent/Diffuse");
	}

	public GameController.direction GetAction (GameMaze m, GameController c, xyLoc myLoc)
	{
		timeElapsed += Time.deltaTime;

		//		Ghosts are forced to reverse direction by the system anytime the mode changes from:
		//		chase-to-scatter
		//		chase-to-frightened
		//		scatter-to-chase
		//		scatter-to-frightened
		if ((lastMode == ghostMode.kChase && currentMode == ghostMode.kScatter) ||
		    (lastMode == ghostMode.kChase && currentMode == ghostMode.kFrightened) ||
		    (lastMode == ghostMode.kScatter && currentMode == ghostMode.kChase) ||
		    (lastMode == ghostMode.kScatter && currentMode == ghostMode.kFrightened)) {
			lastMode = currentMode;
			lastDirection = c.InvertDirection (lastDirection);
			return lastDirection;
		}			

		if (!c.AtIntersection (myLoc)) {
			return lastDirection;
		}

		GameController.direction forbiddenDirection = c.InvertDirection (lastDirection);


		int targetx, targety;
		switch (currentMode) {
		case ghostMode.kChase:
			if (true) { // Implement: when do you leave chase mode?
				lastMode = currentMode;
				//currentMode = // What mode do you change to?
				//currentMode = ghostMode.kScatter;

				timeElapsed = 0;
			}
			GetTarget (out targetx, out targety, c);
			lastDirection = MoveToTarget (targetx, targety, myLoc, forbiddenDirection, c);
			return lastDirection;

		case ghostMode.kScatter:
			if (true) { // Implement: when do you leave chase mode?
				lastMode = currentMode;
				//currentMode = // What mode do you change to?
				currentMode = ghostMode.kChase;

				timeElapsed = 0;
			}
			GetTarget (out targetx, out targety, c);
			lastDirection = MoveToTarget (targetx, targety, myLoc, forbiddenDirection, c);
			return lastDirection;

		case ghostMode.kDead: 
			GetTarget (out targetx, out targety, c);

			// Reached home location
			if (myLoc.x / 8 == targetx && myLoc.y / 8 == targety) {
				lastMode = currentMode;
				//currentMode = // What mode do you change to?
				currentMode = ghostMode.kInHouse;
				this.GetComponent<Renderer> ().material.color = this.GetComponent<GhostSpecialization> ().GetBaseColor ();
				this.transform.localScale = Vector3.one;


				timeElapsed = 0;
			} else {
				lastDirection = MoveToTarget (targetx, targety, myLoc, forbiddenDirection, c);
			}
			return lastDirection;

		case ghostMode.kFrightened:
			lastDirection = MakeRandomAction (myLoc, forbiddenDirection, c);
			return lastDirection;

		case ghostMode.kLeavingHouse:
			// Ready to leave home area
			GetTarget (out targetx, out targety, c);
			if (myLoc.x / 8 == targetx && myLoc.y / 8 == targety) {
				lastMode = currentMode;
				//currentMode = // What mode do you change to?
				currentMode = ghostMode.kScatter;

				timeElapsed = 0;
			} else {
				lastDirection = MoveToTarget (targetx, targety, myLoc, forbiddenDirection, c);
			}
			return lastDirection;

		case ghostMode.kInHouse:
			if (timeElapsed > this.GetComponent<GhostSpecialization> ().GetStartDelay ()) {
				lastMode = currentMode;
				//currentMode = // What mode do you change to?
				currentMode = ghostMode.kLeavingHouse;
				timeElapsed = 0;
			}
			break;
		}

		// Should never get here
		return GameController.direction.kNone;
	}


	void GetTarget (out int x, out int y, GameController c)
	{
		this.GetComponent<GhostSpecialization> ().GetTarget (out x, out y, currentMode, c);
	}

	public bool IsAlive ()
	{
		return currentMode != ghostMode.kDead;
	}

	public void Kill ()
	{
		Color currColor = Color.white;
		currColor.a = 0.5f;
		this.transform.localScale = Vector3.one * 0.5f;
		this.GetComponent<Renderer> ().material.color = currColor;
		//currentMode = // What mode do you change to?
		currentMode = ghostMode.kInHouse;
	}

	public void Reset ()
	{
		this.GetComponent<Renderer> ().material.color = this.GetComponent<GhostSpecialization> ().GetBaseColor ();
		lastDirection = GameController.direction.kLeft;
		currentMode = lastMode = this.GetComponent<GhostSpecialization> ().GetStartMode ();
		transform.localScale = Vector3.one;

		timeElapsed = 0;
	}

	public void StartPowerup ()
	{
		if (currentMode == ghostMode.kDead || currentMode == ghostMode.kInHouse || currentMode == ghostMode.kLeavingHouse)
			return;
		this.GetComponent<Renderer> ().material.color = new Color (0.5f, 0.5f, 1.0f);
		lastMode = currentMode;
		//currentMode = // What mode do you change to?
		currentMode = ghostMode.kFrightened;
	}

	public void EndPowerup ()
	{
		if (currentMode == ghostMode.kDead || currentMode == ghostMode.kInHouse || currentMode == ghostMode.kLeavingHouse)
			return;
		this.GetComponent<Renderer> ().material.color = this.GetComponent<GhostSpecialization> ().GetBaseColor ();
		lastMode = currentMode;
		//currentMode = // What mode do you change to?
		currentMode = ghostMode.kChase;
		timeElapsed = 0;

	}

	public float GetSpeed ()
	{
		if (currentMode == ghostMode.kFrightened)
			return 0.6f;
		else if (currentMode == ghostMode.kDead)
			return 2.0f;
		return 1.0f;
	}

	public static float Dist (int x1, int y1, int x2, int y2)
	{
		return Mathf.Sqrt ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
	}

	GameController.direction MoveToTarget (int targetx, int targety, xyLoc myLoc,
	                                       GameController.direction forbiddenDirection,
	                                       GameController c)
	{
		GameController.direction best = GameController.direction.kUp;
		float distance = 1000f;

		if (c.CanMove (myLoc, GameController.direction.kUp) && forbiddenDirection != GameController.direction.kUp) {
			float thisDist = Dist (targetx, targety, myLoc.x / 8, myLoc.y / 8 + 1);
			if (thisDist < distance) {
				best = GameController.direction.kUp;
				distance = thisDist;
			}
		}
		if (c.CanMove (myLoc, GameController.direction.kLeft) && forbiddenDirection != GameController.direction.kLeft) {
			float thisDist = Dist (targetx, targety, myLoc.x / 8 - 1, myLoc.y / 8);
			if (thisDist < distance) {
				best = GameController.direction.kLeft;
				distance = thisDist;
			}
		}
		if (c.CanMove (myLoc, GameController.direction.kDown) && forbiddenDirection != GameController.direction.kDown) {
			float thisDist = Dist (targetx, targety, myLoc.x / 8, myLoc.y / 8 - 1);
			if (thisDist < distance) {
				best = GameController.direction.kDown;
				distance = thisDist;
			}
		}
		if (c.CanMove (myLoc, GameController.direction.kRight) && forbiddenDirection != GameController.direction.kRight) {
			float thisDist = Dist (targetx, targety, myLoc.x / 8 + 1, myLoc.y / 8);
			if (thisDist < distance) {
				best = GameController.direction.kRight;
				distance = thisDist;
			}
		}
		return best;
	}

	GameController.direction MakeRandomAction (xyLoc myLoc, GameController.direction forbidden, GameController c)
	{
		while (true) {
			switch (Random.Range (0, 4)) {
			case 0:
				if (c.CanMove (myLoc, GameController.direction.kUp) && forbidden != GameController.direction.kUp)
					return GameController.direction.kUp;
				break;
			case 1:
				if (c.CanMove (myLoc, GameController.direction.kLeft) && forbidden != GameController.direction.kLeft)
					return GameController.direction.kLeft;
				break;
			case 2:
				if (c.CanMove (myLoc, GameController.direction.kDown) && forbidden != GameController.direction.kDown)
					return GameController.direction.kDown;
				break;
			case 3:	
				if (c.CanMove (myLoc, GameController.direction.kRight) && forbidden != GameController.direction.kRight)
					return GameController.direction.kRight;
				break;
			}
		}
	}

}
