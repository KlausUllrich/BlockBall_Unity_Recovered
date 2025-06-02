using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class LevelLoader : EditorWindow
{
    //private List<string> levelList = new List<string>();
    private Vector2 _scrollPosition;
	private ListBox _myListBox = new ListBox();

    //-----------------------------------------------------------------------------------------------------------------
    // Add menu item named "LevelLoader" to the BlockBallTools menu
    [MenuItem("BlockBallTools/LevelLoader")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(LevelLoader));
    }

    //-----------------------------------------------------------------------------------------------------------------
    void OnEnable()
    {
        RefreshFileList();
    }
    	
    //-----------------------------------------------------------------------------------------------------------------
    void OnGUI()
    {
        GUILayout.Label("Chose a level", EditorStyles.boldLabel);
        if (GUI.Button(new Rect(140, 100, 100, 30), "Refresh List"))
        {
            RefreshFileList();
        }
        _myListBox.Draw(new Rect(5, 20, 120, 620), 18, Color.gray, Color.cyan);
		
		var labelString = "Selected level: ";
        if (_myListBox.selectedEntry != null && _myListBox.selectedEntry.data != null)
			labelString += _myListBox.selectedEntry.name + " (" + _myListBox.selectedEntry.data + ")";
		else			
			GUI.enabled = false;
		
		GUI.Label(new Rect(140, 20, 400, 30), labelString);

        if (GUI.Button(new Rect(140, 50, 100, 30), "Load"))
        {
			// ToDo M-Code: so abändern das das aktuelle Level zerstört wird
			/*WorldState.GetLevel().*/((Level)_myListBox.selectedEntry.data).Destroy();
			((Level)_myListBox.selectedEntry.data).Build();
        }
		
		GUI.enabled = true;
    }
	
    //-----------------------------------------------------------------------------------------------------------------
    void RefreshFileList()
	{
		_myListBox.Clear();
    
        var levelSetFilePath = Definitions.relLevelFolder + "campain." + Definitions.LevelSetListFileExtention;
        
		var campain = new Campain(levelSetFilePath);
		
		var fileInfo = Definitions.relLevelFolder.GetFiles("*." + Definitions.LevelInfoFileExtention);
		var customLevels = new List<Level>();
		foreach (var file in fileInfo)
		{
			bool found = false;

			// filter campainlevels
			foreach (var levelSet in campain.LevelSets)
			{
				foreach (var level in levelSet.Levels)
				{						
					if (level.FileName == file.Name.Replace("." + Definitions.LevelSetListFileExtention, ""))
					{
						found = true;
						break;
					}
				}
			}
			if (found) continue;

			var customLevel = new Level(file.Name);
			customLevels.Add(customLevel);
		}
		
		// Fill the campain levels in the listbox
		if (campain.LevelSets.Count != 0)
		{
			_myListBox.AddEntry("CAMPAIN_LEVELS");
			foreach (var levelSet in campain.LevelSets)
			{
				_myListBox.AddEntry(" -> " + levelSet.Name.ToUpper());
				foreach (var level in levelSet.Levels)
				{
					if (File.Exists(Definitions.relLevelFolder + level.FileName + "." + Definitions.LevelFileExtention))
						_myListBox.AddEntry("    -> " + level.LevelInfo.LevelName, level);
					else
						_myListBox.AddEntry("    -> " + level.LevelInfo.LevelName + "(file not found!)", level);
				}
			}
		}
		
		// Fill the custom levels in the listbox
		if (customLevels.Count != 0)
		{
			_myListBox.AddEntry("CUSTOM_LEVELS");
			foreach (var level in customLevels)
			{
				_myListBox.AddEntry(" -> " + level.LevelInfo.LevelName, level);
			}
		}
	}

    //-----------------------------------------------------------------------------------------------------------------
}