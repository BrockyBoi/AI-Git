using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkyController : MonoBehaviour, GhostSpecialization
{
	public GhostController.ghostMode GetStartMode ()
	{
		return GhostController.ghostMode.kInHouse;
	}

	public int GetStartDelay ()
	{
		// How long before ghost starts
		return 0;
	}

	public Color GetBaseColor ()
	{
		return new Color (0.5f, 1.0f, 1.0f);
	}

	public void GetTarget (out int x, out int y, GhostController.ghostMode currentMode, GameController c)
	{
		// default code that needs to be written
		x = y = 0;
		switch (currentMode) {
		case GhostController.ghostMode.kChase:
			// Where do you go in chase mode?
			break;
		case GhostController.ghostMode.kScatter:
			// Where do you go in scatter mode?
			x = 0;
			y = 244;
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

