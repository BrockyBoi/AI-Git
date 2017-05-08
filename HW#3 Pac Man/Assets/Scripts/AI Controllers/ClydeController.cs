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
		return 0;
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
			if (GhostController.Dist ((int)transform.position.x, (int)transform.position.z, 
				    GameController.controller.pacmanLoc.x, GameController.controller.pacmanLoc.y) < 8) {
				x = GameController.controller.pacmanLoc.x;
				y = GameController.controller.pacmanLoc.y;
			} else
				currentMode = GhostController.ghostMode.kScatter;
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

