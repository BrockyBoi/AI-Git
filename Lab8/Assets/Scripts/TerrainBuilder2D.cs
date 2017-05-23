using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuilder2D : MonoBehaviour {
	const int width = 1024;
	const int height = 700;
	const float scale = 768;

	int[] terrain;

	void MakeMeshFromTerrain()
	{
		MeshFilter mf = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		mf.mesh = mesh;

		Vector3[] vertices = new Vector3[width*2];
		Vector2[] textureCoordinates = new Vector2[width*2];

		for (int x = 0; x < width; x++)
		{
			vertices[x] = new Vector3((float)x/scale, (float)terrain[x]/scale, 0);
			vertices[x+width] = new Vector3((float)x/scale, 0, 0);
			textureCoordinates [x] = new Vector2 ((float)x / width, terrain [x] / scale);
			textureCoordinates [x+width] = new Vector2 ((float)x / width, 0);
		}
		mesh.vertices = vertices;
		mesh.uv = textureCoordinates;

		int[] triangles = new int[width*6];

		for (int x = 0; x < width-1; x++)
		{
			triangles[x*6+0] = x;
			triangles[x*6+1] = x+1;
			triangles[x*6+2] = x+width;
			triangles[x*6+3] = x+width;
			triangles[x*6+4] = x+1;
			triangles[x*6+5] = x+width+1;
		}
		mesh.triangles = triangles;

		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}
		

	void RandomTerrain()
	{
        for(int x = 0; x < width; x++)
        {
            terrain[x] = Random.Range(0, height);
        }
        for (int i = 0; i < 300; i++)
        {
            for (int x = 0; x < width; x++)
            {
                terrain[x] = (terrain[x] + terrain[(x + 1) % width]) / 2;
            }
        }
	}

	void RandomLinearInterpolateTerrain()
	{
        int step = 128;
        int mask = ~0x7F;
        for(int x = 0; x < width; x += step)
        {
            terrain[x] = Random.Range(0, height);
        }

        for(int x = 0; x < width; x++)
        {
            if (0 == x % step)
                continue;
            int prev = terrain[x & mask];
            int next = terrain[((x & mask) + step) % width];
            float perc = x % step;
            perc /= step;
            terrain[x] = (int)((1 - perc) * prev + (perc) * next);
        }
	}


	void RandomCubicInterpolateTerrain()
	{
        int step = 128;
        int mask = ~0x7F;
        for (int x = 0; x < width; x += step)
        {
            terrain[x] = Random.Range(0, height);
        }

        for (int x = 0; x < width; x++)
        {
            if (0 == x % step)
                continue;
            int prev = terrain[x & mask];
            int next = terrain[((x & mask) + step) % width];
            float perc = x % step;
            perc /= step;
            perc = s(perc);
            terrain[x] = (int)((1 - perc) * prev + (perc) * next);
        }
    }
		
	float s(float x)
	{
		return x * x * (3 - 2 * x);
	}

	void RandomSlopeTerrain()
	{
        terrain[0] = Random.Range(0, height);
        int slope = 2;
        int dist = 8;
        for(int x = 1; x < width; x++)
        {
            terrain[x] = Mathf.Clamp(terrain[x - 1] + slope, 0, height);
            dist--;
            if(dist == 0)
            {
                dist = Random.Range(10, 20);
                slope = Random.Range(-2,3);
            }
        }
	}
		
	void RandomSlopeGradientTerrain()
	{
		terrain [0] = Random.Range (0, height);
		int slopeChange = 2;
		int slope = 2;
		int dist = 2;
		for (int x = 1; x < width; x++)
		{
			terrain [x] = Mathf.Clamp (terrain [x - 1] + slope, 0, height);
			if (terrain [x] == 0 || terrain [x] == height)
			{
				slopeChange = 0;
				slope = 0;
			}

			dist--;
			if (dist == 0)
			{
				dist = Random.Range (1, 5);
				slopeChange = Random.Range (-1, 2);
				slope += slopeChange;
				slope = Mathf.Clamp (slope, -3, 3);
			}
		}
	}

	void FractalTerrain()
	{
        terrain[0] = Random.Range(0, height);
        terrain[width - 1] = Random.Range(0, height);
        SplitTerrain(0, width - 1);
	}

	void SplitTerrain(int start, int end)
	{
        if (start >= end - 1)
            return;
        int range = (end - start) / 5;
        int middle = (end - start) / 2 + start;
        terrain[middle] = (terrain[start] + terrain[end]) / 2 + Random.Range(-range, range);
        terrain[middle] = Mathf.Clamp(terrain[middle], 0, height);
        SplitTerrain(start, middle);
        SplitTerrain(middle, end);
	}

	// Use this for initialization
	void Start () {
		terrain = new int[width];
		RandomTerrain();
		MakeMeshFromTerrain ();
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R))
		{
			RandomTerrain ();
			MakeMeshFromTerrain ();
		}
		if (Input.GetKeyDown (KeyCode.L))
		{
			RandomLinearInterpolateTerrain ();
			MakeMeshFromTerrain ();
		}
		if (Input.GetKeyDown (KeyCode.C))
		{
			RandomCubicInterpolateTerrain ();
			MakeMeshFromTerrain ();
		}
		if (Input.GetKeyDown (KeyCode.S))
		{
			RandomSlopeTerrain ();
			MakeMeshFromTerrain ();
		}
		if (Input.GetKeyDown (KeyCode.G))
		{
			RandomSlopeGradientTerrain ();
			MakeMeshFromTerrain ();
		}

		if (Input.GetKeyDown (KeyCode.F))
		{
			FractalTerrain ();
			MakeMeshFromTerrain ();
		}

	}
}
