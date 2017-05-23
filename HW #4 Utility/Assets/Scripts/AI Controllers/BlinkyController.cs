using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyController : MonoBehaviour, GhostSpecialization
{
	public Color GetBaseColor()
	{
		return Color.red;
	}

	public int GetStartDelay()
	{
		return 0;
	}

	public GhostController.ghostMode GetStartMode()
	{
		return GhostController.ghostMode.kScatter;
	}

	public void GetTarget (out int x, out int y, GhostController.ghostMode currentMode, GameController c)
	{
		switch (currentMode)
		{
		case GhostController.ghostMode.kChase:
			y = c.pacmanLoc.y / 8;
			x = c.pacmanLoc.x / 8;
			break;
		case GhostController.ghostMode.kScatter:
			y = GameMaze.boardHeight - 1;
			x = GameMaze.boardWidth - 3;
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
