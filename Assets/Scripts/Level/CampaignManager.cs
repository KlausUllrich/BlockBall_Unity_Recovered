/*
--- YAML HEADER ---
Intent: Manages the loading and accessibility of the game's campaign structure.
  It serves as the central point for accessing lists of levels, organized into sets (e.g., beginner, easy), 
  as defined in the campaign_definition.xml file.
Architecture: This is a static class providing global access to the campaign data.
  - Initialization: Lazily loads and deserializes 'campaign_definition.xml' from the 
    'Assets/Resources/Levels/' directory upon first access or explicit call to Initialize().
  - Data Storage: Holds the deserialized campaign data in a public static 'Campaign' property 
    of type CampaignDefinition.
  - Access Methods: Provides methods like GetLevelSet() and GetCampaignLevelByFileName() 
    for easy retrieval of campaign parts.
Relations:
  - Loads: 'Assets/Resources/Levels/campaign_definition.xml'
  - Deserializes into: CampaignDefinition.cs (which in turn uses CampaignLevelSet.cs and CampaignLevel.cs)
  - Provides data to: 
    - LevelSelectionManager.cs (or similar UI scripts) for populating level selection screens.
    - Game logic that needs to look up level details by name or set.
---
*/
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using AssemblyCSharp.LevelData; // For CampaignDefinition and other data classes

namespace AssemblyCSharp
{
    public static class CampaignManager
    {
        public static CampaignDefinition Campaign { get; private set; }
        private static bool _isInitialized = false;
        private const string CampaignDefinitionFileName = "campaign_definition"; // Name of the XML file without extension

        public static bool IsInitialized => _isInitialized;

        // Call this method at game startup, e.g., from a GameManager or similar central script.
        public static void Initialize()
        {
            if (_isInitialized) return;

            string resourcePath = "Levels/" + CampaignDefinitionFileName;
            TextAsset campaignTextAsset = Resources.Load<TextAsset>(resourcePath);

            if (campaignTextAsset == null)
            {
                Debug.LogError($"Campaign definition file not found at Resources path: {resourcePath}.xml. Ensure the file exists.");
                Campaign = new CampaignDefinition(); // Initialize with an empty campaign to prevent null refs
                _isInitialized = true; // Mark as initialized even if file not found to avoid repeated load attempts
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(CampaignDefinition));
            using (StringReader reader = new StringReader(campaignTextAsset.text))
            {
                try
                {
                    Campaign = (CampaignDefinition)serializer.Deserialize(reader);
                    Debug.Log("Campaign definition loaded successfully.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error deserializing campaign definition from {resourcePath}.xml: {ex.Message}");
                    Campaign = new CampaignDefinition(); // Initialize with an empty campaign on error
                }
            }
            _isInitialized = true;
        }

        // Optional: Method to get a specific level set by name
        public static CampaignLevelSet GetLevelSet(string setName)
        {
            if (!_isInitialized) Initialize(); // Ensure initialized
            return Campaign?.LevelSets.Find(ls => ls.Name == setName);
        }

        // Optional: Method to get a specific level by filename from any set
        public static CampaignLevel GetCampaignLevelByFileName(string fileName)
        {
            if (!_isInitialized) Initialize(); // Ensure initialized
            foreach (var levelSet in Campaign.LevelSets)
            {
                var level = levelSet.Levels.Find(l => l.FileName == fileName);
                if (level != null) return level;
            }
            return null;
        }
    }
}
