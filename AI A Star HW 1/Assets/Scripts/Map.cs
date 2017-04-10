using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map : MonoBehaviour {

	bool[,] map;

	// Create random obstacles in the map
	void Start () {
		map = new bool[50, 50];

		for (int t = 0; t < 400; t++) {
			int xLoc, yLoc;
			do {
				xLoc = Random.Range (1, 49);
				yLoc = Random.Range (1, 49);
			} while (map [xLoc, yLoc]);
			GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			cube.transform.position = new Vector3 (xLoc, 0.5f, yLoc);
			map [xLoc, yLoc] = true;
		}
	}

	public bool IsOccupied(int x, int y)
	{
		if (x < 0 || x > 49 || y < 0 || y > 49)
			return true;
		return map [x, y];
	}

	public bool CanMove(int x1, int y1, int x2, int y2)
	{
		if (x1 == x2 || y1 == y2) {
			return !IsOccupied (x1, y1) && !IsOccupied (x2, y2);
		} else {
			return !IsOccupied (x1, y1) && !IsOccupied (x1, y2) &&
			!IsOccupied (x2, y1) && !IsOccupied (x2, y2);
		}
	}
}
