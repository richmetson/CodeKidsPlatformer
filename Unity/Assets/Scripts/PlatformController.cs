﻿using UnityEngine;
using System.Collections;
using LuaInterface;

public class PlatformController : LuaController {

	public bool movePlatform = true;
	public float moveSpeed = 1.0f;
	public float moveDistance = 1.0f;
	public float waitLength = 2.0f;

	private Vector2 nextPosition;
	private bool waiting = false;

	private LuaTable luaInstructions;

	public override void Init() {
		lua["platform"] = this;
		lua.DoString("Start()");
		nextPosition = transform.position;
	}

	public override void Reset() {
		// Reset object to start position & settings.
	}
	

	void Update () {

		if (!paused) {
			//Update loop in Lua machine:
			lua.DoString (string.Format ("Update({0})", Time.deltaTime));

			// Move platform 
			if ((Vector2)transform.position == nextPosition) {
				if (!waiting) {
					lua.DoString ("RunNextInstruction()");
				}
			} else {
				transform.position = Vector3.MoveTowards (transform.position, nextPosition, Time.deltaTime * moveSpeed);
			}
		}
	}

	// Read variables from Lua
	public void UpdateOptionsFromLua() {
		LuaTable luaOptions = lua.GetTable("options");
		moveSpeed = (float)(double)luaOptions["moveSpeed"];
		moveDistance = (float)(double)luaOptions["moveDistance"];
		waitLength = (float)(double)luaOptions["waitTime"];
		LuaTable luaColor = (LuaTable)luaOptions["color"];
		UpdateSpriteColors(luaColor);
	}

	// Recieves movement instructions from Lua
	private void Move(string direction) {
		//Debug.Log ("<color=cyan>Next instruction:</color> " + direction);
		switch (direction) {
		case "up":
			nextPosition = transform.position + Vector3.up * moveDistance;
			break;
		case "down":
			nextPosition = transform.position - Vector3.up * moveDistance;
			break;
		case "right":
			nextPosition = transform.position + Vector3.right * moveDistance;
			break;
		case "left":
			nextPosition = transform.position - Vector3.right * moveDistance;
			break;
		case "wait":
			nextPosition = transform.position;
			StartCoroutine("Wait");
			break;
		}
	}

	private IEnumerator Wait() {
		waiting = true;
		yield return new WaitForSeconds(waitLength);
		waiting = false;
	}

	private void UpdateSpriteColors(LuaTable luaColor) {
		float newR = (float)(double)luaColor["r"];
		float newG = (float)(double)luaColor["g"];
		float newB = (float)(double)luaColor["b"];
		float newA = (float)(double)luaColor["a"];
		SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites) {
			sprite.color = new Color(newR, newG, newB, newA);
		}
	}
}
