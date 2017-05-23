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
		width = maxWidth;
		height = maxHeight;
		openClosed = new openItem[width, height];
		round = 1;
	}

	// Initialize any variables specific for this search
	public void Init(xyLoc start, xyLoc goal, Map m, Heuristic heur)
	{
		h = heur;
		g = goal;
		round++;
		success = false;

		AddToOpen (start);
	}

	// Continue the previous search. (Assumes Init is called first.)
	public bool Step(xyLoc start, xyLoc goal, Map m)
	{
		xyLoc next = GetNextToExpand ();

		// Found optimal path
		if (next == goal) {
			success = true;
			return true;
		}

		// Exhausted open list
		if (next.x == -1)
			return true;

		// Continue search
		Expand(m, next);
		AddToClosed (next);

		return false;
	}

	// Find complete path from start to goal using incremental search
	// functions defined above
	public List<xyLoc> GetPath(xyLoc start, xyLoc goal, Map m, Heuristic heur)
	{
		if (start == goal)
			return new List<xyLoc>();
		Init (start, goal, m, heur);

		while (Step(start, goal, m) == false) {
		}

		return ExtractPath ();
	}

	// Add the start state to the open list
	void AddToOpen(xyLoc s)
	{
		AddToOpen (s, 0f, s);
	}

	// Add state to open list. If it is alreay in closed, ignore.
	// If it is already in open, update parent and gCost (if shorter path found)
	// Otherwise add to open
	void AddToOpen(xyLoc s, float gCost, xyLoc parent)
	{
		// check if already here
		if (openClosed [s.x, s.y].round == round) {
			if (openClosed [s.x, s.y].open == false)
				return;
			if (openClosed [s.x, s.y].gCost <= gCost)
				return;
			openClosed [s.x, s.y].gCost = gCost;
			openClosed [s.x, s.y].parent = parent;
			openClosed [s.x, s.y].fCost = gCost+openClosed [s.x, s.y].hCost;
		}
		else {
			float hCost = h.HCost(s, g);
			openClosed [s.x, s.y].gCost = gCost;
			openClosed [s.x, s.y].hCost = hCost;
			openClosed [s.x, s.y].fCost = gCost+hCost;
			openClosed [s.x, s.y].parent = parent;
			openClosed [s.x, s.y].round = round;
			openClosed [s.x, s.y].open = true;
		}
	}

	// Move from open to closed
	void AddToClosed(xyLoc s)
	{
		openClosed [s.x, s.y].open = false;
	}

	void Expand(Map m, xyLoc s)
	{
		xyLoc tmp = new xyLoc ();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;
				float cost = 1.5f;
				if (x == 0 || y == 0)
					cost = 1.0f;
				tmp.x = s.x + x;
				tmp.y = s.y + y;
				if (m.CanMove(s.x, s.y, s.x+x, s.y+y)) {
					AddToOpen (tmp, openClosed [s.x, s.y].gCost + cost, s);
				}
			}
		}
	}

	// Find the next best state to expand
	xyLoc GetNextToExpand()
	{
		xyLoc best = new xyLoc(-1,-1);
		bool valid = false;
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++) {
				if (openClosed [x, y].round == round && openClosed [x, y].open) {
					if (valid == false) {
						best.x = x;
						best.y = y;
						valid = true;
					} else if (openClosed [x, y].fCost < openClosed [best.x, best.y].fCost) {
						best.x = x;
						best.y = y;
					}
					else if (openClosed [x, y].fCost == openClosed [best.x, best.y].fCost) {
						if (openClosed [x, y].gCost > openClosed [best.x, best.y].gCost) {
							best.x = x;
							best.y = y;
						}
					}
				}
			}
		}
		return best;
	}

	// Extract the path from the given state to the goal.
	// (Only runs if search successfully found the goal.)
	public List<xyLoc> ExtractPath()
	{
		List<xyLoc> result = new List<xyLoc> ();
		if (!success)
			return result;
		xyLoc s = g;

		while (openClosed [s.x, s.y].parent != s) {
			result.Add (s);
			s = openClosed [s.x, s.y].parent;
		}
		return result;
	}

	public void DebugDraw()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (openClosed [x, y].round == round) {
					if (openClosed [x, y].open) {
						Debug.DrawLine (new Vector3 (x, 1.0f, y),
							new Vector3 (openClosed [x, y].parent.x, 1.0f, openClosed [x, y].parent.y), Color.black);
					} else {
						Debug.DrawLine (new Vector3 (x, 1.0f, y),
							new Vector3 (openClosed [x, y].parent.x, 1.0f, openClosed [x, y].parent.y), Color.white);
					}
				}
			}
		}
	}
}
