using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkyController : MonoBehaviour, GhostSpecialization
{
	public Color GetBaseColor()
	{
		return new Color (1.0f, 0.5f, 1.0f);
	}


	public int GetStartDelay()
	{
		return 3;
	}

	public GhostController.ghostMode GetStartMode()
	{
		return GhostController.ghostMode.kInHouse;
	}


	public void GetTarget (out int x, out int y, GhostController.ghostMode currentMode, GameController c)
	{
		switch (currentMode)
		{
		case GhostController.ghostMode.kChase:
			y = c.pacmanLoc.y / 8;
			x = c.pacmanLoc.x / 8;
			if (c.pacmanLastMove == GameController.direction.kUp)
				y += 4;
			if (c.pacmanLastMove == GameController.direction.kDown)
				y -= 4;
			if (c.pacmanLastMove == GameController.direction.kLeft)
				x -= 4;
			if (c.pacmanLastMove == GameController.direction.kRight)
				x += 4;
			break;
		case GhostController.ghostMode.kScatter:
			y = GameMaze.boardHeight - 1;
			x = 2;
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

