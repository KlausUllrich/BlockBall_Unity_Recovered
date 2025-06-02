using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

/// <summary>
/// Helper script to complete the PowerUI removal process.
/// Add this to an empty GameObject in your scene and click the buttons in the Inspector.
/// </summary>
public class PowerUICleanupHelper : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private bool powerUIDefineSymbolRemoved = false;
    [SerializeField] private bool powerUIFolderDeleted = false;
    [SerializeField] private bool uiManagerReplaced = false;
    [SerializeField] private bool htmlFilesRemoved = false;
    
    /// <summary>
    /// Step 1: Add DISABLE_POWERUI scripting define symbol (if not already done)
    /// </summary>
    [ContextMenu("1. Add DISABLE_POWERUI Symbol")]
    public void AddDisablePowerUISymbol()
    {
        Debug.Log("Please manually add the DISABLE_POWERUI symbol:");
        Debug.Log("1. Go to Edit > Project Settings > Player");
        Debug.Log("2. Find 'Scripting Define Symbols' under 'Other Settings'");
        Debug.Log("3. Add: DISABLE_POWERUI");
        Debug.Log("4. Click Apply and let Unity recompile");
        
        powerUIDefineSymbolRemoved = false;
    }
    
    /// <summary>
    /// Step 2: Check if Unity UI packages are installed and provide installation instructions if needed
    /// </summary>
    [ContextMenu("2. Check UI Packages")]
    public void CheckUIPackages()
    {
        Debug.Log("Please ensure these Unity packages are installed:");
        Debug.Log("1. Open Unity Package Manager (Window > Package Manager)");
        Debug.Log("2. Click the '+' button > 'Add package by name'");
        Debug.Log("3. Add these packages one by one:");
        Debug.Log("   - com.unity.ugui (Unity UI)");
        Debug.Log("   - com.unity.textmeshpro (TextMeshPro)");
    }
    
    /// <summary>
    /// Step 3: Delete the PowerUI folder
    /// </summary>
    [ContextMenu("3. Delete PowerUI Folder")]
    public void DeletePowerUIFolder()
    {
        string powerUIPath = Path.Combine(Application.dataPath, "PowerUI");
        
        if (Directory.Exists(powerUIPath))
        {
            Debug.Log("PowerUI folder exists at: " + powerUIPath);
            Debug.Log("Please delete it manually in the Unity Project panel:");
            Debug.Log("1. Find and select the 'PowerUI' folder under Assets");
            Debug.Log("2. Right-click and select Delete");
            Debug.Log("3. Confirm the deletion");
        }
        else
        {
            Debug.Log("PowerUI folder already deleted or not found at: " + powerUIPath);
            powerUIFolderDeleted = true;
        }
    }
    
    /// <summary>
    /// Step 4: Check if UiManager has been replaced
    /// </summary>
    [ContextMenu("4. Check UiManager Replacement")]
    public void CheckUiManagerReplacement()
    {
        // Find instances of UiManager and StandardUIManager
        UiManager[] oldManagers = FindObjectsOfType<UiManager>();
        StandardUIManager[] newManagers = FindObjectsOfType<StandardUIManager>();
        
        if (oldManagers.Length > 0)
        {
            Debug.Log("Found " + oldManagers.Length + " UiManager instances that should be replaced.");
            foreach (UiManager manager in oldManagers)
            {
                Debug.Log("UiManager found on GameObject: " + manager.gameObject.name);
            }
            
            Debug.Log("Please replace these with StandardUIManager or delete them once migration is complete.");
            uiManagerReplaced = false;
        }
        else
        {
            Debug.Log("No UiManager instances found - good!");
            
            if (newManagers.Length == 0)
            {
                Debug.LogWarning("No StandardUIManager found either! Make sure to add one to your scene.");
            }
            else
            {
                Debug.Log("Found " + newManagers.Length + " StandardUIManager instances - migration successful!");
                uiManagerReplaced = true;
            }
        }
    }
    
    /// <summary>
    /// Step 5: Remove HTML files from Resources/GUI
    /// </summary>
    [ContextMenu("5. Check HTML Files")]
    public void CheckHTMLFiles()
    {
        string guiPath = Path.Combine(Application.dataPath, "Resources", "GUI");
        
        if (Directory.Exists(guiPath))
        {
            string[] htmlFiles = Directory.GetFiles(guiPath, "*.html", SearchOption.AllDirectories);
            
            if (htmlFiles.Length > 0)
            {
                Debug.Log("Found " + htmlFiles.Length + " HTML files in Resources/GUI that can be removed:");
                foreach (string file in htmlFiles)
                {
                    Debug.Log("HTML file: " + file);
                }
                
                Debug.Log("Please delete these files manually once you've confirmed the UI migration is working.");
                htmlFilesRemoved = false;
            }
            else
            {
                Debug.Log("No HTML files found in Resources/GUI - good!");
                htmlFilesRemoved = true;
            }
        }
        else
        {
            Debug.Log("Resources/GUI folder not found. No HTML files to worry about.");
            htmlFilesRemoved = true;
        }
    }
    
    /// <summary>
    /// Step 6: Remove DISABLE_POWERUI symbol once migration is complete
    /// </summary>
    [ContextMenu("6. Remove DISABLE_POWERUI Symbol")]
    public void RemoveDisablePowerUISymbol()
    {
        if (powerUIFolderDeleted && uiManagerReplaced && htmlFilesRemoved)
        {
            Debug.Log("Migration appears to be complete. You can now remove the DISABLE_POWERUI symbol:");
            Debug.Log("1. Go to Edit > Project Settings > Player");
            Debug.Log("2. Find 'Scripting Define Symbols' under 'Other Settings'");
            Debug.Log("3. Remove: DISABLE_POWERUI");
            Debug.Log("4. Click Apply and let Unity recompile");
            
            powerUIDefineSymbolRemoved = true;
        }
        else
        {
            Debug.LogWarning("Migration is not complete yet! Please complete these steps first:");
            if (!powerUIFolderDeleted) Debug.LogWarning("- Delete PowerUI folder");
            if (!uiManagerReplaced) Debug.LogWarning("- Replace UiManager with StandardUIManager");
            if (!htmlFilesRemoved) Debug.LogWarning("- Remove HTML files from Resources/GUI");
        }
    }
    
    /// <summary>
    /// Step 7: Final cleanup - remove guide scripts
    /// </summary>
    [ContextMenu("7. Final Cleanup")]
    public void FinalCleanup()
    {
        if (powerUIFolderDeleted && uiManagerReplaced && htmlFilesRemoved && powerUIDefineSymbolRemoved)
        {
            Debug.Log("Migration is complete! You can now remove these guide files:");
            Debug.Log("- PowerUIRemovalGuide.cs");
            Debug.Log("- PowerUIRemovalSteps.cs");
            Debug.Log("- PowerUICleanupHelper.cs (this file)");
            Debug.Log("- UISetupHelper.cs (once UI is fully set up)");
            Debug.Log("- LegacyGUIUpgradeGuide.cs");
            
            Debug.Log("Congratulations on successfully migrating from PowerUI to Unity UI!");
        }
        else
        {
            Debug.LogWarning("Migration is not yet complete! Please finish all steps before final cleanup.");
        }
    }
    
    /// <summary>
    /// Check overall migration progress
    /// </summary>
    [ContextMenu("Check Migration Progress")]
    public void CheckMigrationProgress()
    {
        int completedSteps = 0;
        int totalSteps = 4;
        
        if (powerUIFolderDeleted) completedSteps++;
        if (uiManagerReplaced) completedSteps++;
        if (htmlFilesRemoved) completedSteps++;
        if (powerUIDefineSymbolRemoved) completedSteps++;
        
        float progress = (float)completedSteps / totalSteps * 100f;
        
        Debug.Log($"PowerUI Migration Progress: {progress:F1}% ({completedSteps}/{totalSteps} steps completed)");
        
        Debug.Log("Status of each step:");
        Debug.Log($"- PowerUI folder deleted: {(powerUIFolderDeleted ? "✓" : "✗")}");
        Debug.Log($"- UiManager replaced: {(uiManagerReplaced ? "✓" : "✗")}");
        Debug.Log($"- HTML files removed: {(htmlFilesRemoved ? "✓" : "✗")}");
        Debug.Log($"- DISABLE_POWERUI symbol removed: {(powerUIDefineSymbolRemoved ? "✓" : "✗")}");
    }
}
