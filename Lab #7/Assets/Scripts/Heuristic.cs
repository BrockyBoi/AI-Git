using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Heuristic {
	float HCost (xyLoc s, xyLoc g);
}


public class Octile : Heuristic {
	public float HCost (xyLoc s, xyLoc g)
	{
		int minv = Mathf.Min(Mathf.Abs(s.x-g.x), Mathf.Abs(s.y-g.y));
		int maxv = Mathf.Max(Mathf.Abs(s.x-g.x), Mathf.Abs(s.y-g.y));
		return minv * 0.5f + maxv;
	}
}

public class Euclidean : Heuristic {
	public float HCost (xyLoc s, xyLoc g)
	{
		return Mathf.Sqrt ((s.x - g.x) * (s.x - g.x) + (s.y - g.y) * (s.y - g.y));
	}
}
