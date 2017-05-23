using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkyController : MonoBehaviour, GhostSpecialization
{
	public int GetStartDelay()
	{
		return 6;
	}
	public GhostController.ghostMode GetStartMode()
	{
		return GhostController.ghostMode.kInHouse;
	}

	public Color GetBaseColor()
	{
		return new Color (0.5f, 1.0f, 1.0f);
	}

	public void GetTarget (out int x, out int y, GhostController.ghostMode currentMode, GameController c)
	{
		switch (currentMode)
		{
		case GhostController.ghostMode.kChase:
			y = c.pacmanLoc.y / 8;
			x = c.pacmanLoc.x / 8;
			if (c.pacmanLastMove == GameController.direction.kUp)
				y += 2;
			if (c.pacmanLastMove == GameController.direction.kDown)
				y -= 2;
			if (c.pacmanLastMove == GameController.direction.kLeft)
				x -= 2;
			if (c.pacmanLastMove == GameController.direction.kRight)
				x += 2;
			x += 2 * (c.blinkyLoc.x - x);
			y += 2 * (c.blinkyLoc.y - y);
			break;
		case GhostController.ghostMode.kScatter:
			y = 0;
			x = GameMaze.boardWidth - 2;
			break;
		case GhostController.ghostMode.kDead: // back in ghost box
			x = 13;
			y = 21;
			break;
		case GhostController.ghostMode.kLeavingHouse: // back in ghost box
			x = 13;
			y = 21;
			break;
		default: // no target
			x = y = 0;
			break;
		}
	}

}

