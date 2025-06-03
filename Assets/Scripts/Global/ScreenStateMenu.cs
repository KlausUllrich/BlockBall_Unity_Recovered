using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenStateMenu : ScreenStateBase 
{
	public ScreenStateMenu(ScreenStateManager.ScreenStates xType, ScreenStateManager xScreenStateManager)
		: base(xType, xScreenStateManager)
	{
	}
		
	// -------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Show ()
	{
		// Switch to the main menu screen using the new UI system
		m_xScreenStateManager.m_xStandardUIManager.ChangeScreen("MainMenu");

		// Find button components and add listeners
		Button playButton = m_xScreenStateManager.m_xStandardUIManager.GetElementById("PlayButton")?.GetComponent<Button>();
		if (playButton != null)
		{
			playButton.onClick.RemoveAllListeners();
			playButton.onClick.AddListener(Play);
		}
		else
		{
			Debug.LogError("Play button not found in MainMenu screen");
		}

		Button exitButton = m_xScreenStateManager.m_xStandardUIManager.GetElementById("ExitButton")?.GetComponent<Button>();
		if (exitButton != null)
		{
			exitButton.onClick.RemoveAllListeners();
			exitButton.onClick.AddListener(Exit);
		}
		else
		{
			Debug.LogError("Exit button not found in MainMenu screen");
		}
	}

	// -------------------------------------------------------------------------------------------
	public override void Hide()
	{

	}

	// -------------------------------------------------------------------------------------------
	// Update is called once per frame
	public override void Update ()
	{
	
	}

	// -------------------------------------------------------------------------------------------
	void Play()
	{
		m_xScreenStateManager.ChangeToScreenState(ScreenStateManager.ScreenStates.Loding);
	}

	// -------------------------------------------------------------------------------------------
	void Exit()
	{
		Debug.Log("Exit button clicked!");
		
#if UNITY_EDITOR
		// In editor, stop play mode
		UnityEditor.EditorApplication.isPlaying = false;
#else
		// In built application, quit the game
		Application.Quit();
#endif
	}

	// -------------------------------------------------------------------------------------------
}
