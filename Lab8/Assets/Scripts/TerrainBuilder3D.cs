using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuilder3D : MonoBehaviour {
	const int resolution = 129;
	int[,] terrain;

	[Range(0.00001F, 0.025F)]
	public float lowFrequencyPerlin = 0.025f;
	[Range(0.025F, 0.5F)]
	public float highFrequencyPerlin = 0.1f;
	[Range(0.0F, 1.0F)]
	public float highFrequencyPerlinMagnitude = 0.1f;
	[Range(0.25F, 4.0F)]
	public float randomDisplacement = 1.0f;

	float c_lowFreqPerlin, c_highFreqPerlin, c_PerlinMag, c_randDisplace;

	// Use this for initialization
	void Start () {
		terrain = new int[resolution, resolution];
		c_lowFreqPerlin = lowFrequencyPerlin;
		c_highFreqPerlin = highFrequencyPerlin;
		c_randDisplace = randomDisplacement;
		c_PerlinMag = highFrequencyPerlinMagnitude;
		WavyTerrain ();
		MakeMeshFromTerrain ();
	}


	int GetIndexFromXY(int x, int y)
	{
		return x * resolution + y;
	}

	void MakeMeshFromTerrain()
	{
		MeshFilter mf = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		mf.mesh = mesh;

		Vector3[] vertices = new Vector3[resolution*resolution];
		Vector2[] textureCoordinates = new Vector2[resolution*resolution];
		for (int x = 0; x < resolution; x++)
		{
			for (int y = 0; y < resolution; y++)
			{
				int index = GetIndexFromXY (x, y);
				vertices[index] = new Vector3((float)x/resolution, 0.25f*(float)terrain[x,y]/resolution, (float)y/resolution);
				textureCoordinates [x] = new Vector2 (0, (float)terrain[x,y]/resolution);
			}
		}
		mesh.vertices = vertices;
		mesh.uv = textureCoordinates;

		int[] triangles = new int[(resolution-1)*(resolution-1)*6];

		int next = 0;
		for (int x = 0; x < resolution - 1; x++)
		{
			for (int y = 0; y < resolution-1; y++)
			{
				int p1 = GetIndexFromXY (x, y);
				int p2 = GetIndexFromXY (x+1, y);
				int p3 = GetIndexFromXY (x, y+1);
				int p4 = GetIndexFromXY (x+1, y+1);
				triangles [next + 0] = p4;
				triangles [next + 1] = p2;
				triangles [next + 2] = p1;
				triangles [next + 3] = p3;
				triangles [next + 4] = p4;
				triangles [next + 5] = p1;
				next += 6;
			}
		}
		mesh.triangles = triangles;

		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}

    // Take the point in the middle and and compute the average from the surrounding square
    int DiamondAverage(int x, int y, int offset)
    {
        int half = offset / 2;
        int avg = 0;
        int count = 0;

        int spot1 = GetTerrainValue(x - half, y - half);
        int spot2 = GetTerrainValue(x + half, y + half);
        int spot3 = GetTerrainValue(x + half, y - half);
        int spot4 = GetTerrainValue(x - half, y + half);
        avg += (spot1 + spot2 + spot3 + spot4);
        count += 4;

        return avg / count + Random.Range(-3,3);
    }

	// Takes the point on the square and computes the average from the surrounding diamond
	int SquareAverage(int x, int y, int offset)
	{
        //x = 0, y = size / 2, both += size;
        //y = 0, x = size / 2, both += size;
        int avg = 0;
        int half = offset / 2;

        int spot1 = GetTerrainValue(x - half, y);
        int spot2 = GetTerrainValue(x + half, y);
        int spot3 = GetTerrainValue(x, y - half);
        int spot4 = GetTerrainValue(x, y + half);
        avg += (spot1 + spot2 + spot3 + spot4);

        return avg / 4 + Random.Range(-3,3);
	}

    int GetTerrainValue(int x, int y)
    {
        if (x < 0 || x >= resolution || y < 0 || y >= resolution)
        {
            return 0;
        }
        return terrain[x, y];
    }
    int GetTerrainValue(int x, int y, out bool success)
    {
        if(x < 0 || x >= resolution || y < 0 || y >= resolution)
        {
            success = false;
            return 0;
        }
        success = true;
        return terrain[x, y];
    }
	void DiamondSquare()
	{	
        //Diamond -> 4 corners to middle
        //Sauare -> sides from cardinal neighbors

		int step = resolution - 1;

		// init 4 corners
		terrain [0, 0] = Random.Range (0, resolution);
		terrain [0, step] = Random.Range (0, resolution);
		terrain [step, 0] = Random.Range (0, resolution);
		terrain [step, step] = Random.Range (0, resolution);
		do
		{
            for (int x = step / 2; x < resolution; x += step)
            {
                for (int y = step / 2; y < resolution; y += step)
                {
                    terrain[x, y] = DiamondAverage(x, y, step);
                }
            }

            for (int x = 0; x < resolution; x += step)
            {
                for (int y = step / 2; y < resolution; y += step)
                {
                    terrain[x, y] = SquareAverage(x, y, step);
                }
            }

            for (int x = step / 2; x < resolution; x += step)
            {
                for (int y = 0; y < resolution; y += step)
                {
                    terrain[x, y] = SquareAverage(x, y, step);
                }
            }


            step /= 2;
		} while (step >= 2);
	}


	void WavyTerrain()
	{
		for (int x = 0; x < resolution; x++)
		{
			for (int y = 0; y < resolution; y++)
			{
				terrain [x, y] = (int)(resolution*(1f+Mathf.Sin ((x + y)*0.075f))/2f);
			}
		}
	}

	void PerlinTerrain()
	{
		for (int x = 0; x < resolution; x++)
		{
			for (int y = 0; y < resolution; y++)
			{
                terrain[x, y] = (int)(resolution * Mathf.PerlinNoise(x * lowFrequencyPerlin, y * lowFrequencyPerlin));
			}
		}

        //int maxVal = 0, minVal = 0xFFFF;
        //for(int x = 0; x < resolution; x++)
        //{
        //    for(int y = 0; y < resolution; y++)
        //    {
        //        terrain[x, y] = (int)(resolution * Mathf.PerlinNoise(x * lowFrequencyPerlin, y * lowFrequencyPerlin));
        //        terrain[x, y] += (int)(highFrequencyPerlinMagnitude * resolution * Mathf.PerlinNoise(x * highFrequencyPerlin, y * highFrequencyPerlin));
        //        minVal = Mathf.Min(terrain[x, y], minVal);
        //        maxVal = Mathf.Max(terrain[x, y], maxVal);
        //    }
        //}

        //for(int  x= 0; x < resolution; x++)
        //{

        //}
	}

	void OnGUI() {
		if (c_lowFreqPerlin != lowFrequencyPerlin)
		{
			c_lowFreqPerlin = lowFrequencyPerlin;
			PerlinTerrain ();
			MakeMeshFromTerrain ();
		}
		if (c_highFreqPerlin != highFrequencyPerlin)
		{
			c_highFreqPerlin = highFrequencyPerlin;
			PerlinTerrain ();
			MakeMeshFromTerrain ();
		}
		if (c_PerlinMag != highFrequencyPerlinMagnitude)
		{
			c_PerlinMag = highFrequencyPerlinMagnitude;
			PerlinTerrain ();
			MakeMeshFromTerrain ();
		}

		// For DiamondSquare algorithm
		if (c_randDisplace != randomDisplacement)
		{		
			c_randDisplace = randomDisplacement;
			DiamondSquare ();
			MakeMeshFromTerrain ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.W))
		{
			WavyTerrain ();
			MakeMeshFromTerrain ();
		}
		if (Input.GetKeyDown (KeyCode.T))
		{
			DiamondSquare ();
			MakeMeshFromTerrain ();
		}
		if (Input.GetKeyDown (KeyCode.P))
		{
			PerlinTerrain ();
			MakeMeshFromTerrain ();
		}
	}
}
