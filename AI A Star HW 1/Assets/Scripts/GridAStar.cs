using System.Collections.Generic;
using UnityEngine;

public class GridAStar
{

    /*
	 * openItem: Private data structure with items needed inside the open/closed list.
	 */
    struct openItem
    {
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
        round++;
        g = goal;
        h = heur;
        success = false;
        AddToOpen(start);
    }

    // Continue the previous search. (Assumes Init is called first.)
    // Returns true when search is complete
    public bool Step(xyLoc start, xyLoc goal, Map m)
    {
        xyLoc nodeToExplore = GetNextToExpand();
        if (nodeToExplore.x != -1 && nodeToExplore != g)
        {
            Expand(m, nodeToExplore);
            AddToClosed(nodeToExplore);
        }
        else if (nodeToExplore.x == -1)
        {
            success = false;
            return true;
        }
        else if (nodeToExplore == g)
        {
            success = true;
            return true;
        }

        //Get best from open    
        // Expand best
        //Put best on closed
        // Write code here.

        return false;
    }

    // Find complete path from start to goal using incremental search
    // functions defined above
    public List<xyLoc> GetPath(xyLoc start, xyLoc goal, Map m, Heuristic heur)
    {
        if (start == goal)
            return new List<xyLoc>();
        Init(start, goal, m, heur);

        while (Step(start, goal, m) == false) { }

        return ExtractPath();
    }

    // Add the start state to the open list
    void AddToOpen(xyLoc s)
    {
        AddToOpen(s, 0f, s);
    }

    // Add state to open list. If it is alreay in closed, ignore.
    // If it is already in open, update parent and gCost (if shorter path found)
    // Otherwise add to open
    void AddToOpen(xyLoc s, float gCost, xyLoc parent)
    {
        // Write code here.
        if (!openClosed[s.x, s.y].open && openClosed[s.x, s.y].round == round)
        {
            return;
        }
        else if (openClosed[s.x, s.y].round != round)
        {
            openClosed[s.x, s.y].open = true;

            openClosed[s.x, s.y].parent = parent;

            openClosed[s.x, s.y].gCost = gCost;
            openClosed[s.x, s.y].hCost = h.HCost(s, g);

            openClosed[s.x, s.y].fCost = openClosed[s.x, s.y].gCost + openClosed[s.x, s.y].hCost;

            openClosed[s.x, s.y].round = round;

        }
        else if (openClosed[s.x, s.y].open && openClosed[s.x, s.y].round == round)
        {
            float fCos = gCost + openClosed[s.x, s.y].hCost;

            if (fCos < openClosed[s.x, s.y].fCost)
            {
                //Debug.Log("Previous gCost: " + openClosed[s.x,s.y].gCost + " vs. new gCost: " + gCost);
                openClosed[s.x, s.y].parent = parent;
                openClosed[s.x, s.y].fCost = fCos;
                openClosed[s.x, s.y].gCost = gCost;
            }
        }
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
        float gCost = 0;

        for (int i = 0; i < 8; i++)
        {
            xyLoc nextSpot = s;
            switch (i)
            {
                case 0:
                    nextSpot.y = s.y + 1;
                    gCost = 1;
                    break;
                case 1:
                    nextSpot.y = s.y - 1;
                    gCost = 1;
                    break;
                case 2:
                    nextSpot.x = s.x + 1;
                    gCost = 1;
                    break;
                case 3:
                    nextSpot.x = s.x - 1;
                    gCost = 1;
                    break;
                case 4:
                    nextSpot.y = s.y + 1;
                    nextSpot.x = s.x + 1;
                    gCost = 1.5f;
                    break;
                case 5:
                    nextSpot.y = s.y + 1;
                    nextSpot.x = s.x - 1;
                    gCost = 1.5f;
                    break;
                case 6:
                    nextSpot.y = s.y - 1;
                    nextSpot.x = s.x + 1;
                    gCost = 1.5f;
                    break;
                case 7:
                    nextSpot.y = s.y - 1;
                    nextSpot.x = s.x - 1;
                    gCost = 1.5f;
                    break;
                default:
                    break;
            }

            if (m.CanMove(s.x, s.y, nextSpot.x, nextSpot.y))
            {
                AddToOpen(nextSpot, gCost + openClosed[s.x, s.y].gCost, s);
            }
        }

    }

    // Find the next best state to expand
    xyLoc GetNextToExpand()
    {
        xyLoc best = new xyLoc(-1, -1);
        float fCost = Mathf.Infinity;
        // Write code here.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (openClosed[x, y].round == round && openClosed[x, y].open && openClosed[x, y].fCost < fCost)
                {
                    fCost = openClosed[x, y].fCost;
                    best.x = x;
                    best.y = y;
                }
            }
        }

        return best;
    }

    // Extract the path from the given state to the goal.
    // (Only runs if search successfully found the goal.)
    public List<xyLoc> ExtractPath()
    {
        List<xyLoc> result = new List<xyLoc>();
        if (!success)
            return result;
        // Write code here.
        xyLoc item = new xyLoc(g.x, g.y);
        result.Add(item);
        xyLoc parentNode;
        do
        {
            parentNode = openClosed[item.x, item.y].parent;
            result.Add(parentNode);
            item.x = parentNode.x;
            item.y = parentNode.y;
        } while (item != openClosed[item.x, item.y].parent);
        return result;
    }

    public void DebugDraw()
    {
        if (round == 0)
            return;
        // Write code here.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (openClosed[x, y].round == round)
                {
                    if (openClosed[x, y].open)
                        Debug.DrawLine(new Vector3(x, 0.5f, y), new Vector3(openClosed[x, y].parent.x, 0.5f, openClosed[x, y].parent.y), Color.red);
                    else Debug.DrawLine(new Vector3(x, 0.5f, y), new Vector3(openClosed[x, y].parent.x, 0.5f, openClosed[x, y].parent.y), Color.blue);
                }
            }

        }

    }
}
