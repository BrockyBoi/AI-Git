using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {
	enum occupancy {
		kWall,
		kDot,
		kPowerUp,
		kEmpty,
		kSpawn,
		kOutOfBounds
	};

	GameMaze maze;
	GameObject[,] objs;
	public GameMaze GetMaze() { return maze; }

	public int ClearCell(int x, int y, out bool dot, out bool powerUp)
	{
		dot = powerUp = false;
		
		if (maze.GetCellType (x, y) == GameMaze.occupancy.kPowerUp)
			powerUp = true;
		else if (maze.GetCellType (x, y) == GameMaze.occupancy.kDot)
			dot = true;
		if (maze.ClearCell (x, y))
		{
			objs [y, x].GetComponentInChildren<MeshRenderer> ().enabled = false;
			return 1;
		}
		return 0;
	}

	public void Reset()
	{
		maze.Reset ();
		for (int y = 0; y < GameMaze.boardHeight; y++)
		{
			for (int x = 0; x < GameMaze.boardWidth; x++)
			{
				if (maze.GetCellType(x, y) == GameMaze.occupancy.kDot || maze.GetCellType(x, y) == GameMaze.occupancy.kPowerUp)
					objs [y, x].GetComponentInChildren<MeshRenderer> ().enabled = true;
			}
		}
	}
	// Use this for initialization
	void Start () {
		objs = new GameObject[GameMaze.boardHeight, GameMaze.boardWidth];
		maze = new GameMaze ();
		for (int x = 0; x < GameMaze.boardWidth; x++)
		{
			for (int z = 0; z < GameMaze.boardHeight; z++)
			{
				switch (maze.GetCellType(x, z))
				{
				case GameMaze.occupancy.kWall:
					{
						GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
						cube.transform.position = new Vector3 (x + 0.5f, 0.5f, z + 0.5f);
						cube.GetComponent<Renderer> ().material.color = Color.blue;
						objs [z, x] = cube;
					}
					break;
				case GameMaze.occupancy.kSpawnWall:
					{
						GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
						cube.transform.position = new Vector3 (x + 0.5f, 0.5f, z + 0.5f);
						cube.transform.localScale = new Vector3 (1f, 1f, 0.25f);
						cube.GetComponent<Renderer> ().material.color = Color.yellow;
						objs [z, x] = cube;
					}
					break;
				case GameMaze.occupancy.kDot:
					{
						GameObject dot = GameObject.CreatePrimitive (PrimitiveType.Sphere);
						dot.transform.position = new Vector3 (x + 0.5f, 0.5f, z + 0.5f);
						dot.transform.localScale = new Vector3 (0.25f, 0.25f, 0.25f);
						dot.GetComponent<Renderer>().material.color = new Vector4(233f/255f, 196f/255f, 184f/255f, 1.0f);
						objs [z, x] = dot;
					}
					break;
				case GameMaze.occupancy.kPowerUp:
					{
						GameObject dot = GameObject.CreatePrimitive (PrimitiveType.Sphere);
						dot.transform.position = new Vector3 (x + 0.5f, 0.5f, z + 0.5f);
						dot.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
						dot.GetComponent<Renderer>().material.color = new Vector4(233f/255f, 196f/255f, 184f/255f, 1.0f);
						objs [z, x] = dot;
					}
					break;
				default:
					break;
				}
			}
		}
	}
}
