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
		
		// Instead of directly trying to find buttons, let the new panel system handle UI interactions
		// The MainMenuUI component wires up the buttons within its own OnEnable method
		
		// Look for MainMenuUI component which will handle button connections
		MainMenuUI mainMenuUI = GameObject.FindObjectOfType<MainMenuUI>();
		if (mainMenuUI == null)
		{
			Debug.LogWarning("MainMenuUI component not found. Button connections will not be automatic.");
		}
		else
		{
			// Add callbacks for game logic if needed
			if (mainMenuUI.playButton != null)
			{
				mainMenuUI.playButton.onClick.AddListener(Play);
			}
			
			if (mainMenuUI.exitButton != null)
			{
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
