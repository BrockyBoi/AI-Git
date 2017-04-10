using System.Collections.Generic;
using UnityEngine;

public class GridAStar {

	/*
	 * openItem: Private data structure with items needed inside the open/closed list.
	 */
	struct openItem {
		public float gCost;
		public float hCost;
		public float fCost;
		public xyLoc parent;
		public int round; // entry is only valid if the stored round is equal to the current round
		public bool open;
	};

	// The map dimensions - used for allocating the size of the openClosed list
	int width, height;
	openItem[,] openClosed;

	// Keeps track of how many pathfinding calls have been made. 
	int round;

	// Goal location
	xyLoc g;

	// Heuristic function
	Heuristic h;

	// Flag to know if path was found
	bool success;

	// Initialize data structures and internal variables here
	public GridAStar(int maxWidth, int maxHeight)
	{
        // Write code here.
        width = maxWidth;
        height = maxHeight;
        openClosed = new openItem[maxWidth, maxHeight];
	}

	// Initialize any variables specific for this search
	public void Init(xyLoc start, xyLoc goal, Map m, Heuristic heur)
	{
		// Write code here.
        g = goal;
        h = heur;
        round = 0;
        AddToOpen(start);
       // Debug.Log(openClosed[0, 0].parent.ToString());

	}

    // Continue the previous search. (Assumes Init is called first.)
    // Returns true when search is complete
    public bool Step(xyLoc start, xyLoc goal, Map m)
    {
        Expand(m, start);

		// Write code here.

		return true;
	}

	// Find complete path from start to goal using incremental search
	// functions defined above
	public List<xyLoc> GetPath(xyLoc start, xyLoc goal, Map m, Heuristic heur)
	{
		if (start == goal)
			return new List<xyLoc>();
		Init (start, goal, m, heur);

		while (Step(start, goal, m) == false) { Debug.Log("Probably stuck here"); }

		return ExtractPath ();
	}

	// Add the start state to the open list
	void AddToOpen(xyLoc s)
	{
        Debug.Log("Add first xyLoc");
		AddToOpen (s, 0f, s);
	}

    // Add state to open list. If it is alreay in closed, ignore.
    // If it is already in open, update parent and gCost (if shorter path found)
    // Otherwise add to open
    void AddToOpen(xyLoc s, float gCost, xyLoc parent)
    {
        // Write code here.
        if (!openClosed[s.x, s.y].open && openClosed[s.x, s.y].fCost > 0)
            return;

        openItem item = openClosed[s.x, s.y];

        item.parent = parent;

        item.gCost = openClosed[parent.x, parent.y].gCost + s.SquaredDistance(parent.x, parent.y);
        item.hCost = h.HCost(s, g);

        float fCost = item.gCost + item.hCost;

        item.round = round;

        if (item.open && item.fCost > fCost)
        {
            item.fCost = fCost;
            item.parent = parent;
        }
        else if (!item.open)
        {
            item.fCost = fCost;
        }

        item.open = true;

        openClosed[s.x, s.y] = item;
        Debug.Log(item.fCost.ToString());
        Debug.Log(openClosed[s.x, s.y].fCost.ToString());
    }
	// Move from open to closed
	void AddToClosed(xyLoc s)
	{
        // Write code here.
        openClosed[s.x, s.y].open = false;
	}

	// Look at the map to find the successors of s
	// (This could have also gone inside the map itself)
	// Use the map functions to check passability
	void Expand(Map m, xyLoc s)
	{
        // Write code here.
        xyLoc nextSpot = s;

        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    nextSpot.y = s.y + 1;
                    break;
                case 1:
                    nextSpot.y = s.y - 1;
                    break;
                case 2:
                    nextSpot.x = s.x + 1;
                    break;
                case 3:
                    nextSpot.x = s.x - 1;
                    break;
                default:
                    break;

            }

            if (!m.IsOccupied(nextSpot.x, nextSpot.y))
                AddToOpen(nextSpot, nextSpot.SquaredDistance(s.x, s.y), s);
        }

        Expand(m, GetNextToExpand());

	}

	// Find the next best state to expand
	xyLoc GetNextToExpand()
	{
		xyLoc best = new xyLoc(-1,-1);
        round++;
        float fCost = Mathf.Infinity;
		// Write code here.
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y< height; y++)
            {
                if(openClosed[x,y].open && openClosed[x,y].fCost < fCost)
                {
                    fCost = openClosed[x, y].fCost;
                    best.x = x;
                    best.y = y;
                    Debug.Log("New best at :" + best.ToString());
                       
                }

            }
        }

        AddToClosed(best);
		return best;
	}

	// Extract the path from the given state to the goal.
	// (Only runs if search successfully found the goal.)
	public List<xyLoc> ExtractPath()
	{
		List<xyLoc> result = new List<xyLoc> ();
        // Write code here.
        xyLoc item = new xyLoc(-1, -1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!openClosed[x, y].open && openClosed[x,y].parent != null)
                {
                    item.x = x;
                    item.y = y;
                    result.Add(item);
                }

            }
        }
        return result;
	}

	public void DebugDraw()
	{
		// Write code here.

	
	}
}
