using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ControlledObject
{
	GameController.direction GetAction (GameMaze m, GameController c, xyLoc yourLoc);

	bool IsAlive ();

	void Kill ();

	void Reset ();

	void StartPowerup ();

	void EndPowerup ();

	float GetSpeed ();
}

public class GameController : MonoBehaviour
{
	public static GameController controller;

	public enum direction
	{
		kUp,
		kDown,
		kLeft,
		kRight,
		kNone}

	;

	enum gameMode
	{
		kWaitStart,
		kNormalGameMode,
		kPowerUpGameMode,
		kLifeOver,
		kLevelComplete}

	;

	enum gameSound
	{
		kStartSound,
		kBackgroundSound,
		kDieSound,
		kEatGhost,
		kEatDot,
		kEatPowerup}

	;

	const int pixelWidth = 224;
	const int pixelHeight = 288;

	gameMode currentGameMode = gameMode.kWaitStart;
	int eatenDots = 0;
	// Timer for managing time since last frame update
	float elapsedTime = 0;
	// Timer for powerup time left
	float powerUpTimeLeft = 0;

	//public AudioClip startSound;

	public GameObject pacman;

	public xyLoc pacmanLoc { get; private set; }

	public direction pacmanLastMove { get; private set; }

	float pacmanElapsed;

	public GameObject blinky;

	public xyLoc blinkyLoc { get; private set; }

	float blinkyElapsed;

	public GameObject pinky;

	public xyLoc pinkyLoc { get; private set; }

	float pinkyElapsed;

	public GameObject inky;

	public xyLoc inkyLoc { get; private set; }

	float inkyElapsed;

	public GameObject clyde;

	public xyLoc clydeLoc { get; private set; }

	float clydeElapsed;

	public GameObject board;

	// Use this for initialization
	void Start ()
	{
		controller = this;
		ResetCharacterLocations	();
	}

	AudioSource GetSound (gameSound s)
	{
		switch (s) {
		case gameSound.kStartSound:
			return GetComponents<AudioSource> () [0];
		case gameSound.kBackgroundSound:
			return GetComponents<AudioSource> () [1];
		case gameSound.kDieSound:
			return GetComponents<AudioSource> () [2];
		case gameSound.kEatGhost:
			return GetComponents<AudioSource> () [3];
		case gameSound.kEatDot:
			return GetComponents<AudioSource> () [4];
		case gameSound.kEatPowerup:
			return GetComponents<AudioSource> () [5];
		}
		return new AudioSource ();
	}

	void ResetCharacterLocations ()
	{
		pacmanLoc = new xyLoc (13 * 8 + 4, 9 * 8);
		blinkyLoc = new xyLoc (13 * 8 + 4, 21 * 8);
		pinkyLoc = new xyLoc (13 * 8 + 4, 18 * 8);
		inkyLoc = new xyLoc (12 * 8, 18 * 8);
		clydeLoc = new xyLoc (15 * 8, 18 * 8);
	
		blinky.GetComponent<ControlledObject> ().Reset ();
		inky.GetComponent<ControlledObject> ().Reset ();
		pinky.GetComponent<ControlledObject> ().Reset ();
		clyde.GetComponent<ControlledObject> ().Reset ();
	}

	public bool AtGridCenter (xyLoc loc)
	{
		return (loc.x % 8 == 0) && (loc.y % 8 == 0);
	}

	public bool AtIntersection (xyLoc loc)
	{
		return (AtGridCenter (loc));
	}

	public direction InvertDirection (direction d)
	{
		switch (d) {
		case direction.kUp:
			return direction.kDown;
		case direction.kDown:
			return direction.kUp;
		case direction.kRight:
			return direction.kLeft;
		case direction.kLeft:
			return direction.kRight;
		default:
			return direction.kNone;
		}
	}

	public bool CanMove (xyLoc loc, direction d)
	{
		GameMaze m = board.GetComponent<BoardController> ().GetMaze ();
		GameMaze.occupancy currCellType, nextCellType;
		switch (d) {
		case direction.kUp:
			if (loc.x % 8 != 0)
				return false;
			if (loc.y % 8 != 0)
				return true;
			currCellType = m.GetCellType (loc.x / 8, loc.y / 8);
			nextCellType = m.GetCellType (loc.x / 8, loc.y / 8 + 1);
			if (currCellType == GameMaze.occupancy.kSpawn && nextCellType == GameMaze.occupancy.kSpawn)
				return true;
			if (currCellType == GameMaze.occupancy.kSpawn && nextCellType == GameMaze.occupancy.kSpawnWall)
				return true;
			if (nextCellType == GameMaze.occupancy.kSpawn || nextCellType == GameMaze.occupancy.kWall ||
			    nextCellType == GameMaze.occupancy.kSpawnWall)
				return false;
			return true;
		case direction.kDown:
			if (loc.x % 8 != 0)
				return false;
			if (loc.y % 8 != 0)
				return true;
			currCellType = m.GetCellType (loc.x / 8, loc.y / 8);
			nextCellType = m.GetCellType (loc.x / 8, loc.y / 8 - 1);
			if (currCellType == GameMaze.occupancy.kSpawn && nextCellType == GameMaze.occupancy.kSpawn)
				return true;
			if (currCellType == GameMaze.occupancy.kSpawn && nextCellType == GameMaze.occupancy.kSpawnWall)
				return true;
			if (nextCellType == GameMaze.occupancy.kSpawn || nextCellType == GameMaze.occupancy.kWall ||
			    nextCellType == GameMaze.occupancy.kSpawnWall)
				return false;
			return true;
		case direction.kRight:
			return ((loc.y % 8 == 0) &&
			(loc.x >= pixelWidth - 8 - 1 || loc.x % 8 != 0 || (m.GetCellType (loc.x / 8 + 1, loc.y / 8) != GameMaze.occupancy.kWall)));
		case direction.kLeft:
			return ((loc.y % 8 == 0) &&
			(loc.x == 0 || loc.x % 8 != 0 || (m.GetCellType (loc.x / 8 - 1, loc.y / 8) != GameMaze.occupancy.kWall)));
		}
		return true;
	}

	void SetInitialLocations ()
	{
		pacman.transform.position = new Vector3 (pacmanLoc.x / 8.0f + 0.5f, 0.5f, pacmanLoc.y / 8.0f + 0.5f);
		blinky.transform.position = new Vector3 (blinkyLoc.x / 8.0f + 0.5f, 0.5f, blinkyLoc.y / 8.0f + 0.5f);
		pinky.transform.position = new Vector3 (pinkyLoc.x / 8.0f + 0.5f, 0.5f, pinkyLoc.y / 8.0f + 0.5f);
		inky.transform.position = new Vector3 (inkyLoc.x / 8.0f + 0.5f, 0.5f, inkyLoc.y / 8.0f + 0.5f);
		clyde.transform.position = new Vector3 (clydeLoc.x / 8.0f + 0.5f, 0.5f, clydeLoc.y / 8.0f + 0.5f);
	}

	xyLoc HandleAction (xyLoc currLoc, ControlledObject obj, bool canEat)
	{
		GameMaze m = board.GetComponent<BoardController> ().GetMaze ();
		GameController.direction act = obj.GetAction (m, this, currLoc);
		switch (act) {
		case direction.kUp:
			if (CanMove (currLoc, direction.kUp)) {
				currLoc.y++;
				if (canEat)
					pacmanLastMove = direction.kUp;
			}
			break;
		case direction.kDown:
			if (CanMove (currLoc, direction.kDown)) {
				currLoc.y--;
				if (canEat)
					pacmanLastMove = direction.kDown;
			}
			break;
		case direction.kRight:
			if (CanMove (currLoc, direction.kRight)) {
				currLoc.x++;
				if (canEat)
					pacmanLastMove = direction.kRight;		
			}
			break;
		case direction.kLeft:
			if (CanMove (currLoc, direction.kLeft)) {
				currLoc.x--;
				if (canEat)
					pacmanLastMove = direction.kLeft;
			}
			break;
		}
		if (currLoc.x < 0)
			currLoc.x = pixelWidth - 9;
		if (currLoc.x >= pixelWidth - 8)
			currLoc.x = 0;
		if (AtGridCenter (currLoc) && canEat) {
			bool powerUp, dot;
			eatenDots += board.GetComponent<BoardController> ().ClearCell (currLoc.x / 8, currLoc.y / 8, out dot, out powerUp);
			if (powerUp) {
				StartPowerUp ();
				GetSound (gameSound.kEatPowerup).Play ();
			} else if (dot) {
				GetSound (gameSound.kEatDot).Play ();
			}
			if (eatenDots == 244) {
				eatenDots = 0;
				currentGameMode = gameMode.kLevelComplete;
				GetSound (gameSound.kBackgroundSound).Stop ();
			}
		}
		return currLoc;
	}


	// Update is called once per frame
	void Update ()
	{
		elapsedTime += Time.deltaTime;
		pacmanElapsed += pacman.GetComponent<ControlledObject> ().GetSpeed () * Time.deltaTime;
		pinkyElapsed += pinky.GetComponent<ControlledObject> ().GetSpeed () * Time.deltaTime;
		inkyElapsed += inky.GetComponent<ControlledObject> ().GetSpeed () * Time.deltaTime;
		blinkyElapsed += blinky.GetComponent<ControlledObject> ().GetSpeed () * Time.deltaTime;
		clydeElapsed += clyde.GetComponent<ControlledObject> ().GetSpeed () * Time.deltaTime;
		switch (currentGameMode) {
		case gameMode.kWaitStart:
			{
				if (elapsedTime < 1 && !GetSound (gameSound.kStartSound).isPlaying)
					GetSound (gameSound.kStartSound).Play ();
				if (elapsedTime > 4) {
					elapsedTime = 0;
					currentGameMode = gameMode.kNormalGameMode;
					GetSound (gameSound.kBackgroundSound).loop = true;
					GetSound (gameSound.kBackgroundSound).Play ();
				}
				break;
			}
		case gameMode.kPowerUpGameMode:
			{
				powerUpTimeLeft -= Time.deltaTime;
				if (powerUpTimeLeft <= 0)
					EndPowerUp ();
				DoGamePlayLogic ();
				break;
			}
		case gameMode.kNormalGameMode:
			{
				DoGamePlayLogic ();
				break;
			}
		case gameMode.kLifeOver:
			{
				if (elapsedTime > 3) {
					elapsedTime = 0;

					currentGameMode = gameMode.kWaitStart;
					ResetCharacterLocations ();
					pacman.transform.localScale = Vector3.one;
				} else {
					pacman.transform.localScale = Vector3.one * ((3.0f - elapsedTime) / 3.0f);
				}
				break;
			}
		case gameMode.kLevelComplete:
			{
				if (elapsedTime > 3) {
					elapsedTime = 0;

					currentGameMode = gameMode.kWaitStart;
					ResetCharacterLocations ();
					board.GetComponent<BoardController> ().Reset ();
				}
				break;
			}
		}
		SetInitialLocations ();
	}

	void DoGamePlayLogic ()
	{
		if (pacmanElapsed > 1.0f / 30.0f) {
			pacmanLoc = HandleAction (pacmanLoc, pacman.GetComponent<ControlledObject> (), true);
			pacmanElapsed -= 1.0f / 30.0f;
		}
		if (blinkyElapsed > 1.0f / 30.0f) {
			blinkyLoc = HandleAction (blinkyLoc, blinky.GetComponent<ControlledObject> (), false);
			blinkyElapsed -= 1.0f / 30.0f;
		}
		if (pinkyElapsed > 1.0f / 30.0f) {
			pinkyLoc = HandleAction (pinkyLoc, pinky.GetComponent<ControlledObject> (), false);
			pinkyElapsed -= 1.0f / 30.0f;
		}
		if (inkyElapsed > 1.0f / 30.0f) {
			inkyLoc = HandleAction (inkyLoc, inky.GetComponent<ControlledObject> (), false);
			inkyElapsed -= 1.0f / 30.0f;
		}
		if (clydeElapsed > 1.0f / 30.0f) {
			clydeLoc = HandleAction (clydeLoc, clyde.GetComponent<ControlledObject> (), false);
			clydeElapsed -= 1.0f / 30.0f;
		}

		CheckPacmanCollision (blinkyLoc, blinky.GetComponent<ControlledObject> ());
		CheckPacmanCollision (pinkyLoc, pinky.GetComponent<ControlledObject> ());
		CheckPacmanCollision (inkyLoc, inky.GetComponent<ControlledObject> ());
		CheckPacmanCollision (clydeLoc, clyde.GetComponent<ControlledObject> ());
	}

	void CheckPacmanCollision (xyLoc ghost, ControlledObject obj)
	{
		if (!obj.IsAlive ())
			return;
		if ((pacmanLoc.x + 4) / 8 == (ghost.x + 4) / 8 && (pacmanLoc.y + 4) / 8 == (ghost.y + 4) / 8) {
			if (currentGameMode == gameMode.kPowerUpGameMode) {
				obj.Kill ();
				GetSound (gameSound.kEatGhost).Play ();
			} else {
				currentGameMode = gameMode.kLifeOver;
				elapsedTime = 0;
				GetSound (gameSound.kBackgroundSound).Stop ();
				GetSound (gameSound.kDieSound).Play ();
			}
		}
	}



	void StartPowerUp ()
	{
		// Exact timing is missing from PacMan Dossier
		powerUpTimeLeft = 10f;
		currentGameMode = gameMode.kPowerUpGameMode;
		blinky.GetComponent<ControlledObject> ().StartPowerup ();
		inky.GetComponent<ControlledObject> ().StartPowerup ();
		pinky.GetComponent<ControlledObject> ().StartPowerup ();
		clyde.GetComponent<ControlledObject> ().StartPowerup ();
	}

	void EndPowerUp ()
	{
		// Exact timing is missing from PacMan Dossier
		powerUpTimeLeft = 0f;
		currentGameMode = gameMode.kNormalGameMode;
		blinky.GetComponent<ControlledObject> ().EndPowerup ();
		inky.GetComponent<ControlledObject> ().EndPowerup ();
		pinky.GetComponent<ControlledObject> ().EndPowerup ();
		clyde.GetComponent<ControlledObject> ().EndPowerup ();
	}


}
