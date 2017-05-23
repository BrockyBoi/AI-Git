using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManController : MonoBehaviour, ControlledObject {
	GameController.direction lastDirection;
	int [,] distances;
	bool powerUp;
	int lastx, lasty;
	int dotx, doty, pelletx, pellety;
    int powerUpsGotten;
    float nextAction;
    xyLoc lastClosestGhostLoc;
    public enum GraphType
    {
        Linear, Quadratic, Logistic
    }
    [Range(0,1)]
    public float ChaseGhostM, GhostFleeM, ChaseDotM, ChasePowerPelletM;
	void Start () {
		powerUp = false;
		distances = new int[GameMaze.boardWidth,GameMaze.boardHeight];
		lastDirection = GameController.direction.kLeft;
	}

	GameController.direction GetBestNeighborMove(int x, int y)
	{
		if (x > 0 && distances [x - 1, y] == distances [x, y] - 1)
			return GameController.direction.kLeft;
		if (x < GameMaze.boardWidth-1 && distances [x + 1, y] == distances [x, y] - 1)
			return GameController.direction.kRight;
		if (y > 0 && distances [x , y-1] == distances [x, y] - 1)
			return GameController.direction.kDown;
		if (y < GameMaze.boardHeight-1 && distances [x, y+1] == distances [x, y] - 1)
			return GameController.direction.kUp;
		return GameController.direction.kNone;
	}

	int GetBestNeighborDist(int x, int y)
	{
		int best = 1000;
		if (x > 0)
			best = Mathf.Min (best, distances [x - 1, y]);
		if (x < GameMaze.boardWidth-1)
			best = Mathf.Min (best, distances [x + 1, y]);
		if (y > 0)
			best = Mathf.Min (best, distances [x, y-1]);
		if (y < GameMaze.boardHeight-1)
			best = Mathf.Min (best, distances [x, y+1]);
		return best;
	}

	void AddNeighbors(Queue<xyLoc> q, GameMaze m, int x, int y)
	{
		if (m.CanMove (x, y, x + 1, y))
			q.Enqueue (new xyLoc (x + 1, y));
		if (m.CanMove(x, y, x-1, y))
			q.Enqueue (new xyLoc (x-1, y));
		if (m.CanMove(x, y, x, y+1))
			q.Enqueue (new xyLoc (x, y+1));
		if (m.CanMove(x, y, x, y-1))
			q.Enqueue (new xyLoc (x, y-1));		
	}

    float GetDirectionUtility(GameController c, GameMaze m, GameController.direction dir)
    {
        xyLoc ghostLoc = GetClosestGhostLoc(c);
        float util = 0;
        int x = c.pacmanLoc.x / 8;
        int y = c.pacmanLoc.y / 8;
        xyLoc nextSpot = new xyLoc();
        switch (dir)
        {
            case GameController.direction.kLeft:
                {
                    nextSpot = new xyLoc(x - 1, y);
                    if (ghostLoc.x >= x)
                    {
                        util += GetGhostFleeValue(c) * GhostFleeM;
                    }
                }
                break;
            case GameController.direction.kRight:
                {
                    nextSpot = new xyLoc(x + 1, y);
                    if (ghostLoc.x <= x)
                    {
                        util += GetGhostFleeValue(c) * GhostFleeM;
                    }
                }
                break;
            case GameController.direction.kDown:
                {
                    nextSpot = new xyLoc(x, y - 1);
                    if (ghostLoc.y >= y)
                    {
                        util += GetGhostFleeValue(c) * GhostFleeM;
                    }
                }
                break;
            case GameController.direction.kUp:
                {
                    nextSpot = new xyLoc(x, y + 1);
                    if (ghostLoc.y <= y)
                    {
                        util += GetGhostFleeValue(c) * GhostFleeM;
                    }
                }
                break;
            default:
                break;
        }

        if (m.GetCellType(nextSpot) == GameMaze.occupancy.kWall)
            return -1;

        if(powerUp)
        {
            util += GetChaseGhostValue(c) * ChaseGhostM;
        }

        if (MoveTowards(nextSpot) != MoveTowards(ghostLoc))
        {
            util += GetGhostFleeValue(c) * GhostFleeM;
        }

        if (MoveTowards(nextSpot) == MoveTowards(dotx, doty))
        {
            util += GetChaseDotValue(c, new xyLoc(dotx, doty)) * ChaseDotM;
        }

        if (powerUpsGotten < 4 && MoveTowards(nextSpot) == MoveTowards(pelletx, pellety))
        {
            util += GetGhostFleeValue(c) * ChasePowerPelletM;
        }
        util = Mathf.Clamp(util, 0, 1);
        return util;
    }

	void GetDistances(GameController c, GameMaze m, int x, int y)
	{
		dotx = doty = 1000;
        pelletx = pellety = 1000;

		for (int xLoc = 0; xLoc < GameMaze.boardWidth; xLoc++)
		{
			for (int yLoc = 0; yLoc < GameMaze.boardHeight; yLoc++)
			{
				distances[xLoc,yLoc] = 1000;
			}
		}

		distances [x, y] = 0;
		// Here we are using xyLoc to store grid cells, not pixel cells

		Queue<xyLoc> q = new Queue<xyLoc> ();
		AddNeighbors (q, m, x, y);
		while (q.Count > 0)
		{
            xyLoc l = q.Dequeue ();
			if (distances[l.x,l.y] == 1000)
			{
				if (dotx == 1000 && (m.GetCellType(l.x, l.y) == GameMaze.occupancy.kDot))
				{
                    dotx = l.x;
                    doty = l.y;
				}
                if(pelletx == 1000 && m.GetCellType(l.x,l.y) == GameMaze.occupancy.kPowerUp)
                {
                    pelletx = l.x;
                    pellety = l.y;
                }
					
				distances [l.x, l.y] = GetBestNeighborDist (l.x, l.y)+1;
				AddNeighbors (q, m, l.x, l.y);
			}
		}
	}

	GameController.direction MoveTowards(int x, int y)
	{
		if (distances [x, y] == 0)
			return lastDirection;
		if (distances [x, y] == 1)
		{
			// Move is to get to our loc; need to reverse it
			switch (GetBestNeighborMove(x, y))
			{
			case GameController.direction.kLeft:
				return GameController.direction.kRight;
			case GameController.direction.kRight:
				return GameController.direction.kLeft;
			case GameController.direction.kUp:
				return GameController.direction.kDown;
			case GameController.direction.kDown:
				return GameController.direction.kUp;
			}
		}
		switch (GetBestNeighborMove(x, y))
		{
		case GameController.direction.kLeft:
			return MoveTowards (x-1, y);
		case GameController.direction.kRight:
			return MoveTowards (x+1, y);
		case GameController.direction.kUp:
			return MoveTowards (x, y + 1);
		case GameController.direction.kDown:
			return MoveTowards (x, y - 1);
		}
		return GameController.direction.kNone;
	}

    GameController.direction MoveTowards(xyLoc loc)
    {
        if (distances[loc.x, loc.y] == 0)
            return lastDirection;
        if (distances[loc.x, loc.y] == 1)
        {
            // Move is to get to our loc; need to reverse it
            switch (GetBestNeighborMove(loc.x, loc.y))
            {
                case GameController.direction.kLeft:
                    return GameController.direction.kRight;
                case GameController.direction.kRight:
                    return GameController.direction.kLeft;
                case GameController.direction.kUp:
                    return GameController.direction.kDown;
                case GameController.direction.kDown:
                    return GameController.direction.kUp;
            }
        }
        switch (GetBestNeighborMove(loc.x, loc.y))
        {
            case GameController.direction.kLeft:
                return MoveTowards(loc.x - 1, loc.y);
            case GameController.direction.kRight:
                return MoveTowards(loc.x + 1, loc.y);
            case GameController.direction.kUp:
                return MoveTowards(loc.x, loc.y + 1);
            case GameController.direction.kDown:
                return MoveTowards(loc.x, loc.y - 1);
        }
        return GameController.direction.kNone;
    }

    public GameController.direction GetAction (GameMaze m, GameController c, xyLoc yourLoc)
	{
        if ((yourLoc.x / 8 != lastx || yourLoc.y / 8 != lasty) &&
            (yourLoc.x % 8 == 0 && yourLoc.y % 8 == 0))
        {
            lastx = yourLoc.x / 8;
            lasty = yourLoc.y / 8;
            GetDistances(c, m, lastx, lasty);

            float lUtil, rUtil, dUtil, uUtil;

            lUtil = GetDirectionUtility(c, m, GameController.direction.kLeft);
            rUtil = GetDirectionUtility(c, m, GameController.direction.kRight);
            dUtil = GetDirectionUtility(c, m, GameController.direction.kDown);
            uUtil = GetDirectionUtility(c, m, GameController.direction.kUp);
            float max = Mathf.NegativeInfinity;
            if (lUtil > max)
            {
                lastDirection = GameController.direction.kLeft;
                max = lUtil;

            }
            if (rUtil > max)
            {
                lastDirection = GameController.direction.kRight;
                max = rUtil;

            }
            if (dUtil > max)
            {
                lastDirection = GameController.direction.kDown;
                max = dUtil;

            }
            if (uUtil > max)
            {
                lastDirection = GameController.direction.kUp;
                max = uUtil;
            }
        }

		return lastDirection;
	}

    #region Utility Functions
    float GetLinearCurveValue(float input, float multiplier)
    {
        return Mathf.Clamp(input * multiplier, 0, 1);
    }

    float GetQuadraticCurveValue(float input, float multiplier, float exponent)
    {
        return Mathf.Clamp(Mathf.Pow(input*multiplier, exponent), 0, 1);
    }

    float GetLogisticCurveValue(float input, float mutliplier)
    {
        return Mathf.Clamp(1 /(1 + Mathf.Pow(2.718f,-input)) , 0, 1);
    }

    float GetChaseGhostValue(GameController c)
    {
        if (!powerUp)
            return 0;

        float closestGhostDistance = 0;
        GetClosestGhostLoc(c, out closestGhostDistance);

        // return 1 - GetQuadraticCurveValue(closestGhostDistance, .01f, 3);
        return 1 - GetLinearCurveValue(closestGhostDistance * 4, .01f);
    }

    float GetGhostFleeValue(GameController c)
    {
        if (powerUp)
            return 0;

        float closestGhostDistance = 0;
        GetClosestGhostLoc(c, out closestGhostDistance);
        //return 1 - GetQuadraticCurveValue(closestGhostDistance * 4, .01f, 3);
        return 1 - GetLinearCurveValue(closestGhostDistance * 4, .01f);
    }

    float GetChaseDotValue(GameController c, xyLoc closestDot)
    {
        return 1 - GetLinearCurveValue(GhostController.Dist(new xyLoc(c.pacmanLoc.x / 8, c.pacmanLoc.y / 8), closestDot) * 4, .01f) - .2f;
    }

    float GetChasePowerPelletValue(GameController c, xyLoc closestPowerPellet)
    {
        if (powerUp)
            return 0;
        //return 1 - GetQuadraticCurveValue(GhostController.Dist(c.pacmanLoc, GetClosestGhostLoc(c)), .01f, 4);
        return 1 - GetLinearCurveValue(GhostController.Dist(new xyLoc(c.pacmanLoc.x / 8, c.pacmanLoc.y / 8), closestPowerPellet) * 4, .01f);
    }

    xyLoc GetClosestGhostLoc(GameController c)
    {
        float minDist = Mathf.Infinity;
        xyLoc pacLoc = c.pacmanLoc; 
        xyLoc returnLoc = new xyLoc();
        float pinkDist, clydeDist, inkDist, blinkDist;
        pinkDist = distances[c.pinkyLoc.x / 8, c.pinkyLoc.y / 8];//GhostController.Dist(pacLoc.x / 8, pacLoc.y / 8, c.pinkyLoc.x / 8, c.pinkyLoc.y / 8);
        clydeDist = distances[c.clydeLoc.x / 8, c.clydeLoc.y / 8];//GhostController.Dist(pacLoc.x / 8, pacLoc.y / 8, c.clydeLoc.x / 8, c.clydeLoc.y / 8);
        inkDist = distances[c.inkyLoc.x / 8, c.inkyLoc.y / 8];//GhostController.Dist(pacLoc.x / 8, pacLoc.y / 8, c.inkyLoc.x / 8, c.inkyLoc.y / 8);
        blinkDist = distances[c.blinkyLoc.x / 8, c.blinkyLoc.y / 8];//GhostController.Dist(pacLoc.x / 8, pacLoc.y / 8, c.blinkyLoc.x / 8, c.blinkyLoc.y / 8);

        if (pinkDist < minDist)
        {
            minDist = pinkDist;
            returnLoc = c.pinkyLoc;
        }
        if (clydeDist < minDist)
        {
            minDist = clydeDist;
            returnLoc = c.clydeLoc;
        }
        if (inkDist < minDist)
        {
            minDist = inkDist;
            returnLoc = c.inkyLoc;
        }
        if (blinkDist < minDist)
        {
            minDist = blinkDist;
            returnLoc = c.blinkyLoc;
        }
        return new xyLoc(returnLoc.x / 8, returnLoc.y / 8);
    }

    xyLoc GetClosestGhostLoc(GameController c, out float distance)
    {
        float minDist = Mathf.Infinity;
        xyLoc pacLoc = c.pacmanLoc;
        xyLoc returnLoc = new xyLoc();
        float pinkDist, clydeDist, inkDist, blinkDist;
        pinkDist = distances[c.pinkyLoc.x / 8, c.pinkyLoc.y / 8];//GhostController.Dist(pacLoc.x / 8, pacLoc.y / 8, c.pinkyLoc.x / 8, c.pinkyLoc.y / 8);
        clydeDist = distances[c.clydeLoc.x / 8, c.clydeLoc.y / 8];//GhostController.Dist(pacLoc.x / 8, pacLoc.y / 8, c.clydeLoc.x / 8, c.clydeLoc.y / 8);
        inkDist = distances[c.inkyLoc.x / 8, c.inkyLoc.y / 8];//GhostController.Dist(pacLoc.x / 8, pacLoc.y / 8, c.inkyLoc.x / 8, c.inkyLoc.y / 8);
        blinkDist = distances[c.blinkyLoc.x / 8, c.blinkyLoc.y / 8];//GhostController.Dist(pacLoc.x / 8, pacLoc.y / 8, c.blinkyLoc.x / 8, c.blinkyLoc.y / 8);

        if (pinkDist < minDist)
        {
            minDist = pinkDist;
            returnLoc = c.pinkyLoc;
        }
        if (clydeDist < minDist)
        {
            minDist = clydeDist;
            returnLoc = c.clydeLoc;
        }
        if (inkDist < minDist)
        {
            minDist = inkDist;
            returnLoc = c.inkyLoc;
        }
        if (blinkDist < minDist)
        {
            minDist = blinkDist;
            returnLoc = c.blinkyLoc;
        }
        distance = minDist;
        return new xyLoc(returnLoc.x / 8, returnLoc.y / 8);
    }

    #endregion

    public bool IsAlive()
	{
		return true;
	}

	public void Kill()
	{
		lastx = lasty = 0;
		powerUp = false;
		lastDirection = GameController.direction.kLeft;
	}

	public void Reset()
	{
		lastx = lasty = 0;
		powerUp = false;
		lastDirection = GameController.direction.kLeft;
	}

	public void StartPowerup()
	{
		powerUp = true;
        powerUpsGotten++;
	}

	public void EndPowerup()
	{
		powerUp = false;
	}
	public float GetSpeed()
	{
		return 1.0f;
	}
}
