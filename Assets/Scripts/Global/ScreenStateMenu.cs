/*---
title: ScreenStateMenu
description: Handles the Menu screen state within the game's screen state system
role: Initializes the main menu screen when the game state is changed to Menu state
relationships:
  - Extends ScreenStateBase
  - Used by ScreenStateManager to control menu state
  - Connects to MainMenuUI for button handlers
components:
  - Uses StandardUIManager to activate MainMenu screen
  - Finds and connects to MainMenuUI buttons for gameplay actions
---*/

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
		// Show the main menu screen directly
		if (m_xScreenStateManager != null && m_xScreenStateManager.m_xStandardUIManager != null) 
		{
			m_xScreenStateManager.m_xStandardUIManager.ChangeScreen("MainMenu");
			
			// Start a coroutine to find and connect to MainMenuUI after the screen is shown
			MonoBehaviour monoBehaviour = m_xScreenStateManager as MonoBehaviour;
			if (monoBehaviour != null)
			{
				monoBehaviour.StartCoroutine(ConnectToMainMenuUI());
			}
		}
		else
		{
			Debug.LogError("ScreenStateManager or StandardUIManager reference is null! Cannot show MainMenu screen.");
		}
	}
	
	// Find MainMenuUI after a delay to allow the screen to activate
	private IEnumerator ConnectToMainMenuUI()
	{
		// Wait for one frame to ensure the MainMenu screen is active
		yield return null;
		
		// Find any MainMenuUI component (active or inactive)
		MainMenuUI mainMenuUI = null;
		
		// First try to find it directly from the MainMenu screen
		Transform mainMenuScreen = m_xScreenStateManager.m_xStandardUIManager.transform.Find("MainMenuScreen");
		if (mainMenuScreen != null)
		{
			// Look in the panels inside MainMenuScreen for MainMenuUI
			Transform mainMenuPanel = mainMenuScreen.Find("MainMenuPanel");
			if (mainMenuPanel != null)
			{
				mainMenuUI = mainMenuPanel.GetComponent<MainMenuUI>();
			}
		}
		
		// If not found, fall back to a general search
		if (mainMenuUI == null)
		{
			mainMenuUI = GameObject.FindObjectOfType<MainMenuUI>();
		}

		if (mainMenuUI == null)
		{
			Debug.LogWarning("MainMenuUI component not found. Button connections will not be automatic.");
		}
		else 
		{
			// Add callbacks for game logic if needed
			if (mainMenuUI.playButton != null)
			{
				mainMenuUI.playButton.onClick.RemoveListener(Play); // Remove first to prevent duplicates
				mainMenuUI.playButton.onClick.AddListener(Play);
			}
			
			if (mainMenuUI.exitButton != null)
			{
				mainMenuUI.exitButton.onClick.RemoveListener(Exit); // Remove first to prevent duplicates
				mainMenuUI.exitButton.onClick.AddListener(Exit);
			}
			
			// No need to add listener for level selection - MainMenuUI handles that
		}
	}

	// -------------------------------------------------------------------------------------------
	public override void Hide()
	{
		// Hide method intentionally left empty as screen hiding is handled by StandardUIManager
	}

	// -------------------------------------------------------------------------------------------
	// Update is called once per frame
	public override void Update ()
	{
		// Update method intentionally left empty
	}

	// -------------------------------------------------------------------------------------------
	void Play()
	{
		// When Play is clicked, we'll load the default level
		BlockMerger blockMerger = Object.FindObjectOfType<BlockMerger>();
		
		// If there's no BlockMerger, create one (this is critical for level loading)
		if (blockMerger == null)
		{
			GameObject mergerObj = new GameObject("BlockMerger");
			blockMerger = mergerObj.AddComponent<BlockMerger>();
			Debug.Log("Created BlockMerger for level loading");
		}
		
		// Set default level if none is set
		if (string.IsNullOrEmpty(blockMerger.LevelToLoad))
		{
			blockMerger.LevelToLoad = "level_01";
			Debug.Log("Set default level to load: level_01");
		}
		
		// Change to loading state
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
