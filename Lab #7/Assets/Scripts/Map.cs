using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map : MonoBehaviour {
	public const int width = 75;
	public const int height = 75;
	public const float scale = 50f / width;
	bool[,] map;
	GameObject[,] walls;
	public Material stone;

    public int xWidth, yWidth, totalSize;
	// Create random obstacles in the map
	void Awake () {
		map = new bool[width, height];
		walls = new GameObject[width, height];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				cube.transform.position = new Vector3 (x*scale, 1.0f+0.5f, y*scale);
				cube.transform.localScale = new Vector3 (50f / width, 2.0f*50f/width, 50f / height);
				cube.GetComponent<Renderer> ().material.color = Color.black;
				cube.GetComponent<Renderer> ().material = stone;
				walls [x, y] = cube;
			}
		}
	
		Reset ();
	}



	public void Reset()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				map [x, y] = true;
			}
		}

		Split (1, width - 2, 1, height - 2, Random.value>0.5f);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				walls[x,y].SetActive (map[x,y]);
			}
		}

	}

	void Fill(Room r)
	{
		for (int x = r.l; x < r.r; x++)
		{
			for (int y = r.b; y < r.t; y++)
			{
				map [x, y] = false;
			}
		}
	}

	Room Split(int x1, int x2, int y1, int y2, bool horiz)
	{
		Room r = new Room ();
		r.Clear ();
		// base case: clear area
		if (Mathf.Abs(x2 - x1 - xWidth * 2) <= totalSize || Mathf.Abs(y2 - y1 - yWidth * 2) <= totalSize) // change this line to decide when to stop splitting
		{
			r.l = x1 + 1;
			r.r = x2;
			r.t = y2;
			r.b = y1 + 1;
			Fill (r);
			return r;
		}
        Room next1, next2;
		if (horiz)
		{
            // choose a splitting point
            int avg = Mathf.RoundToInt((y1 + y2) / 2);
			int meet = Random.Range(avg - 5, avg + 5);// replace the -1
            // Horizontal line, so x coordinates stay the same, y coordinates are split
            // Make 2 calls to split, one one each side, and get the rooms (bounding boxes)
            // for each side of the room

            // Split(...)
            // Split(...)
            next1 = Split(x1 + xWidth, x2 - xWidth, y1 + yWidth, meet - yWidth, Random.value > .5f);
            next2 = Split(x1 + xWidth, x2 - xWidth, meet + yWidth, y2 - yWidth, Random.value > .5f);

            // Draw a line to connect the two boxes together
            // The line should start at the split point and move outward in each direction
            // until it finds a location that is unoccupied
            int avgX = Mathf.RoundToInt((x1 + x2) / 2);
            for(int y = meet; y < height; y++)
            {
                if (IsOccupied(avgX, y))
                {
                    map[avgX, y] = false;
                }
                else break;
            }
            for(int y = meet - 1; y > 0; y--)
            {
                if (IsOccupied(avgX, y))
                {
                    map[avgX, y] = false;
                }
                else break;
            }
			// We merge the two boxes to get the return value
			r = next1 + next2;
			return r;
		}
		else {
            // choose a splitting point
            int avg = Mathf.RoundToInt((x1 + x2) / 2);
            int meet = Random.Range(avg - 5, avg + 5);// replace the -1

            // Vertical line, so y coordinates stay the same, x coordinates are split
            // Make 2 calls to split, one one each side, and get the rooms (bounding boxes)
            // for each side of the room

            // Split(...)
            // Split(...)

            next1 = Split(x1 + xWidth, meet - xWidth, y1 + yWidth, y2 - yWidth, Random.value > .5f);
            next2 = Split(meet + xWidth, x2 - xWidth, y1 + yWidth, y2 - yWidth, Random.value > .5f);

            // Draw a line to connect the two boxes together
            // The line should start at the split point and move outward in each direction
            // until it finds a location that is unoccupied
            int avgY = Mathf.RoundToInt((y1 + y2) / 2);
            for (int x = meet; x < width; x++)
            {
                if (IsOccupied(x, avgY))
                {
                    map[x, avgY] = false;
                }
                else break;
            }
            for (int x = meet - 1; x > 0; x--)
            {
                if (IsOccupied(x, avgY))
                {
                    map[x, avgY] = false;
                }
                else break;
            }
            // We merge the two boxes to get the return value
            r = next1 + next2;
			return r;
		}
		return r;
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.R))
			Reset ();		
	}

	public bool IsOccupied(int x, int y)
	{
		if (x < 0 || x >= width || y < 0 || y >= height)
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
