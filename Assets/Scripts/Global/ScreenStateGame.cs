using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenStateGame : ScreenStateBase 
{		
	public ScreenStateGame(ScreenStateManager.ScreenStates xType, ScreenStateManager xScreenStateManager)
		: base(xType, xScreenStateManager)
	{
	}
		
	// -------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Show () 
	{
		// Switch to game UI screen
		m_xScreenStateManager.m_xStandardUIManager.ChangeScreen("Game");
		WorldStateManager.GamePaused = false;
	}

	// -------------------------------------------------------------------------------------------
	public override void Hide()
	{
		WorldStateManager.GamePaused = true;		
	}

	// -------------------------------------------------------------------------------------------
	// Update is called once per frame
	public override void Update () 
	{
	
	}

	// -------------------------------------------------------------------------------------------
}
