/*
--- YAML HEADER ---
Intent: Represents a set or group of levels within the campaign (e.g., "Beginner", "Easy").
  These sets are used to categorize levels for the player, often corresponding to 
  filters or sections in the level selection UI.
Architecture: This class is a data container designed for XML serialization.
  - It holds a 'Name' (internal identifier, e.g., "beginner") and a 'DisplayName' (for UI, e.g., "Beginner").
  - It contains a list of 'CampaignLevel' objects representing the individual levels in this set.
  - Attributes [XmlAttribute("Name")], [XmlAttribute("DisplayName")], and [XmlElement("Level")] map to
    the corresponding parts of a <LevelSet> element in 'campaign_definition.xml'.
Relations:
  - Contained by: CampaignDefinition.cs (as part of a List<CampaignLevelSet>).
  - Contains: A List<CampaignLevel> (CampaignLevel.cs objects).
  - Serialized from/to: <LevelSet> elements within 'campaign_definition.xml'.
  - Used by: CampaignManager.cs for accessing levels by set, and LevelSelectionManager.cs (or similar UI scripts)
    to display level categories.
---
*/
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AssemblyCSharp.LevelData
{
    public class CampaignLevelSet
    {
        [XmlAttribute("Name")]
        public string Name { get; set; } // Internal identifier

        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; } // Name shown in UI

        [XmlElement("Level")]
        public List<CampaignLevel> Levels { get; set; }

        public CampaignLevelSet()
        {
            Levels = new List<CampaignLevel>();
        }
    }
}
