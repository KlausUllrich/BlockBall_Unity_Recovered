using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Unity UI implementation for intro screens and splash logos.
/// Handles the display of company and game logos during startup.
/// </summary>
public class IntroScreenUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image logoImage; // UnityEngine.UI.Image
    public EventTrigger logoTrigger; // UnityEngine.EventSystems.EventTrigger
    
    private StandardUIManager uiManager;
    private Action onClickAction;
    
    void Awake()
    {
        // Find UI Manager
        uiManager = FindObjectOfType<StandardUIManager>();
        
        // Set up logo click event
        if (logoTrigger == null && logoImage != null)
        {
            // Add event trigger if not already present
            logoTrigger = logoImage.gameObject.GetComponent<EventTrigger>();
            if (logoTrigger == null)
            {
                logoTrigger = logoImage.gameObject.AddComponent<EventTrigger>();
            }
        }
        
        if (logoTrigger != null)
        {
            // Ensure the trigger list exists
            if (logoTrigger.triggers == null)
            {
                logoTrigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
            }
            
            // Clear existing triggers
            logoTrigger.triggers.Clear();
        }
        else
        {
            Debug.LogError("Logo trigger not assigned in IntroScreenUI");
        }
    }
    
    // Set the action to execute when the logo is clicked
    public void SetClickAction(Action action)
    {
        onClickAction = action;
        
        if (logoTrigger != null)
        {
            // Add click event
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { OnLogoClicked(); });
            logoTrigger.triggers.Add(entry);
        }
    }
    
    private void OnLogoClicked()
    {
        // Execute the callback if set
        onClickAction?.Invoke();
    }
}
