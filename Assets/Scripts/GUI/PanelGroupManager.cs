using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages a group of UI panels, ensuring only one is visible at a time.
/// This follows the architectural principle of having a single Canvas with child GameObjects for screens,
/// but adds a layer of management to prevent visibility errors.
/// </summary>
public class PanelGroupManager : MonoBehaviour
{
    [Serializable]
    public class PanelInfo
    {
        public string panelId;
        public GameObject panelObject;
        public UnityEvent onPanelShow;
        public UnityEvent onPanelHide;
    }
    
    [Header("Panel Configuration")]
    public PanelInfo[] panels;
    public string defaultPanelId;
    
    [Header("Events")]
    public UnityEvent<string> onPanelChanged;
    
    private Dictionary<string, PanelInfo> panelMap = new Dictionary<string, PanelInfo>();
    private string currentPanelId;
    
    void Awake()
    {
        InitializePanels();
    }
    
    void Start()
    {
        // Show default panel if specified
        if (!string.IsNullOrEmpty(defaultPanelId) && string.IsNullOrEmpty(currentPanelId))
        {
            ShowPanel(defaultPanelId);
        }
    }
    
    /// <summary>
    /// Initializes the panel dictionary and sets all panels to inactive
    /// </summary>
    public void InitializePanels()
    {
        // Clear the map in case this is called multiple times
        panelMap.Clear();
        
        // Build panel map
        foreach (var panel in panels)
        {
            if (panel.panelObject != null && !string.IsNullOrEmpty(panel.panelId))
            {
                panelMap[panel.panelId] = panel;
                panel.panelObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"Invalid panel configuration: {(panel.panelObject == null ? "Missing GameObject" : "Missing ID")}");
            }
        }
        
        Debug.Log($"PanelGroupManager initialized with {panelMap.Count} panels");
    }
    
    /// <summary>
    /// Shows the specified panel and hides all others
    /// </summary>
    /// <param name="panelId">ID of the panel to show</param>
    /// <returns>True if panel was found and shown</returns>
    public bool ShowPanel(string panelId)
    {
        if (string.IsNullOrEmpty(panelId))
        {
            Debug.LogError("Cannot show panel: panelId is null or empty");
            return false;
        }
        
        // Don't do anything if this panel is already shown
        if (panelId == currentPanelId)
            return true;
            
        // Hide current panel if one is active
        if (!string.IsNullOrEmpty(currentPanelId) && panelMap.TryGetValue(currentPanelId, out PanelInfo currentPanel))
        {
            currentPanel.panelObject.SetActive(false);
            currentPanel.onPanelHide?.Invoke();
        }
        
        // Show requested panel
        if (panelMap.TryGetValue(panelId, out PanelInfo newPanel))
        {
            newPanel.panelObject.SetActive(true);
            newPanel.onPanelShow?.Invoke();
            currentPanelId = panelId;
            onPanelChanged?.Invoke(panelId);
            
            Debug.Log($"Panel changed to: {panelId}");
            return true;
        }
        else
        {
            Debug.LogError($"Panel not found: {panelId}");
            return false;
        }
    }
    
    /// <summary>
    /// Returns to the default panel
    /// </summary>
    public void ShowDefaultPanel()
    {
        if (!string.IsNullOrEmpty(defaultPanelId))
            ShowPanel(defaultPanelId);
    }
    
    /// <summary>
    /// Gets the currently active panel ID
    /// </summary>
    public string GetCurrentPanelId()
    {
        return currentPanelId;
    }
    
    /// <summary>
    /// Check if a panel exists in this manager
    /// </summary>
    public bool HasPanel(string panelId)
    {
        return panelMap.ContainsKey(panelId);
    }
    
    /// <summary>
    /// Gets a panel object by its ID
    /// </summary>
    public GameObject GetPanelObject(string panelId)
    {
        if (panelMap.TryGetValue(panelId, out PanelInfo panel))
            return panel.panelObject;
        return null;
    }
    
    /// <summary>
    /// Registers a new panel at runtime
    /// </summary>
    public void RegisterPanel(string panelId, GameObject panelObject)
    {
        if (string.IsNullOrEmpty(panelId) || panelObject == null)
        {
            Debug.LogError("Cannot register panel: Invalid ID or GameObject");
            return;
        }
        
        if (panelMap.ContainsKey(panelId))
        {
            Debug.LogWarning($"Panel {panelId} already exists. Overwriting.");
        }
        
        PanelInfo newPanel = new PanelInfo
        {
            panelId = panelId,
            panelObject = panelObject,
            onPanelShow = new UnityEvent(),
            onPanelHide = new UnityEvent()
        };
        
        panelMap[panelId] = newPanel;
        panelObject.SetActive(false);
        
        // Resize panels array to include new panel
        Array.Resize(ref panels, panels.Length + 1);
        panels[panels.Length - 1] = newPanel;
        
        Debug.Log($"Panel {panelId} registered successfully");
    }
}
