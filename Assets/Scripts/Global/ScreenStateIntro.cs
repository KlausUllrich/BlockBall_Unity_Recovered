using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenStateIntro : ScreenStateBase 
{
	private float m_fShowDuration = 5.0f; // Time in seconds to show intro screen
	private float m_fTimer = 0.0f;
	
	public ScreenStateIntro(ScreenStateManager.ScreenStates xType, ScreenStateManager xScreenStateManager)
		: base(xType, xScreenStateManager)
	{
		// Using the new UI system with a single intro screen
	}
		
	// -------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Show () 
	{
		// Reset timer when showing the intro screen
		m_fTimer = 0.0f;
		
		// Show the intro screen
		if (m_xScreenStateManager.m_xStandardUIManager != null)
		{
			m_xScreenStateManager.m_xStandardUIManager.ChangeScreen("Intro");
			SetupEventHandler();
		}
		else
		{
			Debug.LogError("StandardUIManager not set in ScreenStateManager!");
			// Skip intro and go to menu if UI manager is missing
			m_xScreenStateManager.ChangeToScreenState(ScreenStateManager.ScreenStates.Menu);
		}
	}

	// -------------------------------------------------------------------------------------------
	public override void Hide()
	{
		// Hide the intro screen
		if (m_xScreenStateManager.m_xStandardUIManager != null)
		{
			// Current screen will be hidden by the UI manager when changing screens
		}
	}

	// -------------------------------------------------------------------------------------------
	public override void Update ()
	{
		// Increment timer
		m_fTimer += Time.unscaledDeltaTime;
		
		// Automatically transition to menu after specified duration
		if (m_fTimer >= m_fShowDuration)
		{
			GoToMenu();
		}
	}

	// -------------------------------------------------------------------------------------------
	// Called when the intro screen is clicked
	public void OnIntroClicked(PointerEventData eventData)
	{
		GoToMenu();
	}

	// -------------------------------------------------------------------------------------------
	// Transition to the menu screen
	public void GoToMenu()
	{
		m_xScreenStateManager.ChangeToScreenState(ScreenStateManager.ScreenStates.Menu);
	}

	// -------------------------------------------------------------------------------------------
	// Set up click event handler on the intro screen
	public void SetupEventHandler()
	{
		// Get the intro screen
		GameObject introScreen = m_xScreenStateManager.m_xStandardUIManager.introScreen;
		if (introScreen == null) return;
		
		// Find the logo object (or use the entire screen if no logo found)
		Transform logoTransform = introScreen.transform.Find("Logo");
		GameObject targetObject = (logoTransform != null) ? logoTransform.gameObject : introScreen;
		
		// Add event trigger component if it doesn't exist
		EventTrigger eventTrigger = targetObject.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = targetObject.AddComponent<EventTrigger>();
		}

		// Clear existing triggers
		eventTrigger.triggers = new List<EventTrigger.Entry>();

		// Add click event
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener((data) => { OnIntroClicked((PointerEventData)data); });
		eventTrigger.triggers.Add(entry);
		
		Debug.Log("Intro screen click handler set up successfully");
	}

	// -------------------------------------------------------------------------------------------
}
