﻿using System.Collections;
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
		return 4;
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
			//Vector2 offset = new Vector2 (c.inkyLoc.x, c.inkyLoc.y) - new Vector2 (c.pacmanLoc.x, c.pacmanLoc.y);
			xyLoc offset = new xyLoc (c.blinkyLoc.x - c.pacmanLoc.x, c.blinkyLoc.y - c.pacmanLoc.y);
                int xOff = 0;
                int yOff = 0;
			switch (c.pacmanLastMove) {
			case GameController.direction.kLeft:
                        xOff = c.pacmanLoc.x -2;
                        yOff = c.pacmanLoc.y;
                    x = (xOff + offset.x) / 8;
				y = (yOff + offset.y) / 8;
				break;
			case GameController.direction.kRight:
                        xOff = c.pacmanLoc.x + 2;
                        yOff = c.pacmanLoc.y;
                        x = (xOff + offset.x) / 8;
                        y = (yOff + offset.y) / 8;
                        break;
			case GameController.direction.kUp:
                        xOff = c.pacmanLoc.x;
                        yOff = c.pacmanLoc.y + 2;
                        x = (xOff + offset.x) / 8;
                        y = (yOff + offset.y) / 8;
                        break;
			case GameController.direction.kDown:
                        xOff = c.pacmanLoc.x;
                        yOff = c.pacmanLoc.y - 2;
                        x = (xOff + offset.x) / 8;
                        y = (yOff + offset.y) / 8;
                        break;
			default:
				break;
			}
			break;
		case GhostController.ghostMode.kScatter:
			// Where do you go in scatter mode?
			y = 0;
			x = 244 / 8;
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

