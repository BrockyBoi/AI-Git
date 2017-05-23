using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaze {
	public enum occupancy {
		kWall,
		kDot,
		kPowerUp,
		kEmpty,
		kSpawn,
		kOutOfBounds,
		kSpawnWall
	};

	public const int boardWidth = 28;
	public const int boardHeight = 36;
	occupancy[,] board;
	int[,] initBoard;

	// Use this for initialization
	public GameMaze () {
		Init();
	}

	public occupancy GetCellType(int x, int y)
	{
		if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
			return occupancy.kOutOfBounds;
		y = boardHeight - y-1;
		return board [y, x];
	}

    public occupancy GetCellType(xyLoc loc)
    {
        if (loc.x < 0 || loc.x >= boardWidth || loc.y < 0 || loc.y >= boardHeight)
            return occupancy.kOutOfBounds;
        loc.y = boardHeight - loc.y - 1;
        return board[loc.y, loc.x];
    }

	public bool ClearCell(int x, int y)
	{
		y = boardHeight - y-1;
		// Only clear objects that can be cleared
		if (board [y, x] == occupancy.kPowerUp || board [y, x] == occupancy.kDot)
		{
			board [y, x] = occupancy.kEmpty;
			return true;
		}
		return false;
	}

	public bool CanMove(int x1, int y1, int x2, int y2)
	{
		y1 = boardHeight - y1-1;
		y2 = boardHeight - y2-1;

		if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 >= boardWidth || x2 >= boardWidth || y1 >= boardHeight || y2 >= boardHeight)
		{
			return false;
		}
		return ((board [y1, x1] == occupancy.kDot ||
				 board [y1, x1] == occupancy.kPowerUp ||
				 board [y1, x1] == occupancy.kEmpty) &&
				(board [y2, x2] == occupancy.kDot ||
				 board [y2, x2] == occupancy.kPowerUp ||
				 board [y2, x2] == occupancy.kEmpty));
	}

	public void Reset()
	{
		for (int y = 0; y < boardHeight; y++) 
		{
			for (int x = 0; x < boardWidth; x++) 
			{
				switch (initBoard [y, x]) 
				{
				case 0:
					board [y, x] = occupancy.kWall;
					break;
				case 1:
					board [y, x] = occupancy.kDot;
					break;
				case 2:
					board [y, x] = occupancy.kPowerUp;
					break;
				case 3:
					board [y, x] = occupancy.kEmpty;
					break;
				case 4:
					board [y, x] = occupancy.kSpawn;
					break;
				case 5:
					board [y, x] = occupancy.kOutOfBounds;
					break;
				case 6:
					board [y, x] = occupancy.kSpawnWall;
					break;
				}
			}
		}
	}

	void Init()
	{
		initBoard = new int[boardHeight, boardWidth] {
			{ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
			{ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
			{ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
			{ 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0 },
			{ 0, 2, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2, 0 },
			{ 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0 },
			{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
			{ 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0 },
			{ 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0 },
			{ 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0 },
			{ 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
			{ 5, 5, 5, 5, 5, 0, 1, 0, 0, 0, 0, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 1, 0, 5, 5, 5, 5, 5 },
			{ 5, 5, 5, 5, 5, 0, 1, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 1, 0, 5, 5, 5, 5, 5 },
			{ 5, 5, 5, 5, 5, 0, 1, 0, 0, 3, 0, 0, 0, 6, 6, 0, 0, 0, 3, 0, 0, 1, 0, 5, 5, 5, 5, 5 },
			{ 0, 0, 0, 0, 0, 0, 1, 0, 0, 3, 0, 0, 0, 4, 4, 0, 0, 0, 3, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
			{ 3, 3, 3, 3, 3, 3, 1, 3, 3, 3, 0, 4, 4, 4, 4, 4, 4, 0, 3, 3, 3, 1, 3, 3, 3, 3, 3, 3 },
			{ 0, 0, 0, 0, 0, 0, 1, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
			{ 5, 5, 5, 5, 5, 0, 1, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 1, 0, 5, 5, 5, 5, 5 },
			{ 5, 5, 5, 5, 5, 0, 1, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 1, 0, 5, 5, 5, 5, 5 },
			{ 5, 5, 5, 5, 5, 0, 1, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 1, 0, 5, 5, 5, 5, 5 },
			{ 0, 0, 0, 0, 0, 0, 1, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
			{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
			{ 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0 },
			{ 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0 },
			{ 0, 2, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 3, 3, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 2, 0 },
			{ 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0 },
			{ 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0 },
			{ 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0 },
			{ 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
			{ 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
			{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
			{ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 }
		};
		board = new occupancy[boardHeight, boardWidth];
		Reset ();
	}
}
