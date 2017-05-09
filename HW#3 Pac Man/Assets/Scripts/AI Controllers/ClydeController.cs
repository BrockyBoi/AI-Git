using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClydeController : MonoBehaviour, GhostSpecialization
{
	public Color GetBaseColor ()
	{
		return new Color (1.0f, 137f / 255f, 38f / 255f);
	}

	public int GetStartDelay ()
	{
		// How long before ghost starts (implement)
		return 8;
	}

	public GhostController.ghostMode GetStartMode ()
	{
		return GhostController.ghostMode.kInHouse;
	}

	public void GetTarget (out int x, out int y, GhostController.ghostMode currentMode, GameController c)
	{
		// default code that needs to be written
		x = y = 0;
		switch (currentMode) {
		case GhostController.ghostMode.kChase:
			// Where do you go in chase mode?
			if (GhostController.Dist (c.clydeLoc.x / 8, c.clydeLoc.y / 8, 
				    c.pacmanLoc.x / 8, c.pacmanLoc.y / 8) < 8) {
				x = c.pacmanLoc.x / 8;
				y = c.pacmanLoc.y / 8;
			} else {
				x = 0;
				y = 0;
			}
			break;
		case GhostController.ghostMode.kScatter:
			x = 0;
			y = 0;
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

