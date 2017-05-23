using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClydeController : MonoBehaviour, GhostSpecialization
{
	public int GetStartDelay()
	{
		return 9;
	}

	public Color GetBaseColor()
	{
		return new Color (1.0f, 137f/255f, 38f/255f);
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
			if (GhostController.Dist(x, y, c.clydeLoc.x/8, c.clydeLoc.y/8) < 8)
			{
				y = x = 0;
			}
			break;
		case GhostController.ghostMode.kScatter:
			y = 0;
			x = 0;
			break;
		case GhostController.ghostMode.kDead:
			x = 13;
			y = 21;
			break;
		case GhostController.ghostMode.kLeavingHouse:
			x = 13;
			y = 21;
			break;
		default: // no target
			x = y = 0;
			break;
		}
	}
}

