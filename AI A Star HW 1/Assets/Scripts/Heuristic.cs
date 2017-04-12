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
        float xDiff = Mathf.Abs(s.x - g.x);
        float yDiff = Mathf.Abs(s.y - g.y);

        return Mathf.Max(xDiff, yDiff) + ((Mathf.Sqrt(2)- 1) * Mathf.Min(xDiff, yDiff));
    }
}

