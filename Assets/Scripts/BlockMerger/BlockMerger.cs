using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class BlockMerger : MonoBehaviour
{
    // -------------------------------------------------------------------------------------------
    public string LevelToLoad;

    // Track the currently loaded level for proper cleanup
    private Level currentLevel;

    // -------------------------------------------------------------------------------------------

    // -------------------------------------------------------------------------------------------
    void Awake()
    {
    }

	// -------------------------------------------------------------------------------------------
	void Start()
	{
     
	}

	// -------------------------------------------------------------------------------------------
	public void LoadLevel()
	{
		// Destroy the previous level if it exists
		if (currentLevel != null)
		{
			currentLevel.Destroy();
			currentLevel = null;
		}
		
		// Create and build the new level
		currentLevel = new Level(LevelToLoad);
		currentLevel.Build();        
	}

    // -------------------------------------------------------------------------------------------
    void OnDestroy()
    {
        // Clean up the current level when BlockMerger is destroyed
        if (currentLevel != null)
        {
            currentLevel.Destroy();
            currentLevel = null;
        }
    }

    // -------------------------------------------------------------------------------------------
}
