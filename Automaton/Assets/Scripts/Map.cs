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

	public void Iterate()
	{
        // Run Automaton 
        bool[,] mapCopy = new bool[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                int count = 0;
                for(int i = -1; i <=1; i++)
                {
                    for(int j = -1; j <= 1; j++)
                    {
                        count += IsOccupied(x + i, y + j) ? 1 : 0;
                    }
                }
                if (count >= 5)
                    mapCopy[x, y] = true;
                else mapCopy[x,y] = false;
            }
        }
        map = mapCopy;
		// Turn on objects to show walls
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				walls[x,y].SetActive (map[x,y]);
			}
		}

	}

	public void Reset()
	{
		// Turn on objects to show walls
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
                map[x, y] = Random.value >= .5f;
                walls[x,y].SetActive (map[x,y]);
			}
		}

	}
		
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.R))
			Reset ();
		
		if (Input.GetKeyDown (KeyCode.I))
			Iterate();
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
