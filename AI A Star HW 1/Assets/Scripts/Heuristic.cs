using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Heuristic
{
    float HCost(xyLoc s, xyLoc g);
}


public class Octile : Heuristic
{
    public float HCost(xyLoc s, xyLoc g)
    {
        return Mathf.Max(Mathf.Abs(s.x - g.x), Mathf.Abs(s.y - g.y)) + (Mathf.Sqrt(2) - 1) * Mathf.Min(Mathf.Abs(s.x - g.x), Mathf.Abs(s.y - g.y));
    }
}

