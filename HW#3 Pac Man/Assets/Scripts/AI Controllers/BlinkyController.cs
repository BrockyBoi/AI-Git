using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyController : MonoBehaviour, GhostSpecialization
{
	public Color GetBaseColor ()
	{
		return Color.red;
	}

	public int GetStartDelay ()
	{
		return 0;
	}

	public GhostController.ghostMode GetStartMode ()
	{
		return GhostController.ghostMode.kScatter;
	}

	public void GetTarget (out int x, out int y, GhostController.ghostMode currentMode, GameController c)
	{
		// default code that needs to be written
		x = y = 0;
		switch (currentMode) {
		case GhostController.ghostMode.kChase:
			// Where do you go in chase mode?
			x = c.pacmanLoc.x / 8;
			y = c.pacmanLoc.y / 8;
			break;
		case GhostController.ghostMode.kScatter:
			x = 244;
			y = 288;
			// Where do you go in scatter mode?
			break;
		case GhostController.ghostMode.kDead: // back in ghost box
			// Moves to top of ghost box before regenerating
			x = 13;
			y = 21;
			break;
		case GhostController.ghostMode.kLeavingHouse: // back in ghost box
			// Moves to top of ghost box before starting regular behavior
			x = 13;
			y = 21;
			break;
		default: // no target
			x = y = 0;
			break;
		}
	}

}
