using UnityEngine;
using System.Collections;

/// <summary>
/// Migration wrapper for UiManager that redirects to StandardUIManager.
/// This class exists only for backward compatibility during migration.
/// It should be removed once all PowerUI references are removed from the project.
/// </summary>
public class UiManager : MonoBehaviour 
{
	// Reference to the new UI manager
	private StandardUIManager standardUIManager;

	// -------------------------------------------------------------------------------------------
	// OnEnable is called when the game starts, or when the manager script component is enabled.
	void OnEnable()
	{
		// Find the StandardUIManager in the scene
		standardUIManager = FindObjectOfType<StandardUIManager>();
		if (standardUIManager == null)
		{
			Debug.LogError("StandardUIManager not found in scene! Please add it to continue migration from PowerUI.");
		}
		else
		{
			Debug.Log("UiManager is now redirecting to StandardUIManager. This is a temporary migration wrapper.");
		}
	}

	// -------------------------------------------------------------------------------------------
	/// <summary>
	/// Migration wrapper method that redirects to StandardUIManager.ChangeScreen
	/// </summary>
	public void ChangeHtml(string resource) 
	{
		if (standardUIManager != null)
		{
			// Convert the HTML resource name to a screen name for StandardUIManager
			standardUIManager.ChangeScreen(resource);
		}
		else
		{
			Debug.LogError("StandardUIManager not found. Cannot change UI screen.");
		}
	}

	// -------------------------------------------------------------------------------------------
	// OnDisable is called when the manager script component is disabled.
	void OnDisable () {
		// No cleanup needed for StandardUIManager
	}
	
}
