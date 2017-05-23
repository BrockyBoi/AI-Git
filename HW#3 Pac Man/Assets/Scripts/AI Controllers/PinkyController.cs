using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkyController : MonoBehaviour, GhostSpecialization
{
	public Color GetBaseColor ()
	{
		return new Color (1.0f, 0.5f, 1.0f);
	}

	public int GetStartDelay ()
	{
		// How long before ghost starts
		return 0;
	}

	public GhostController.ghostMode GetStartMode ()
	{
		// What mode do you start in?
		return GhostController.ghostMode.kInHouse;
	}


	public void GetTarget (out int x, out int y, GhostController.ghostMode currentMode, GameController c)
	{
		// default code that needs to be written
		x = y = 0;
		switch (currentMode) {
		case GhostController.ghostMode.kChase:
			// Where do you go in chase mode?
			switch (c.pacmanLastMove) {
			case GameController.direction.kLeft:
				x = (c.pacmanLoc.x - 4) / 8;
				y = c.pacmanLoc.y / 8;
				break;
			case GameController.direction.kRight:
				x = (c.pacmanLoc.x + 4) / 8;
				y = c.pacmanLoc.y / 8;
				break;
			case GameController.direction.kUp:
				x = c.pacmanLoc.x / 8;
				y = (c.pacmanLoc.y + 4) / 8;
				break;
			case GameController.direction.kDown:
				x = c.pacmanLoc.x / 8;
				y = (c.pacmanLoc.y + 4) / 8;
				break;
			default:
				x = 0;
				y = 288 / 8;
				break;
			}
			break;
		case GhostController.ghostMode.kScatter:
			// Where do you go in scatter mode?
			x = 0;
			y = 288 / 8;
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

