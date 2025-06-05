/*
--- YAML HEADER ---
Intent: Represents a single level entry within a campaign level set.
  It primarily stores the filename of the .level file and a display name for UI purposes.
Architecture: This class is a simple data container designed for XML serialization.
  - It holds a 'FileName' (e.g., "first_roll_campain") which is the name of the .level file 
    (without the .level extension) located in 'Assets/Resources/Levels/'.
  - It holds a 'DisplayName' (e.g., "First Roll") for use in UI elements like level selection buttons.
  - Attributes [XmlAttribute("FileName")] and [XmlAttribute("DisplayName")] map to the
    corresponding attributes of a <Level> element in 'campaign_definition.xml'.
Relations:
  - Contained by: CampaignLevelSet.cs (as part of a List<CampaignLevel>).
  - Serialized from/to: <Level> elements within a <LevelSet> in 'campaign_definition.xml'.
  - Refers to: A .level file in 'Assets/Resources/Levels/' via its FileName property.
  - Used by: LevelSelectionManager.cs (or similar UI scripts) to display individual level entries
    and to know which .level file to load when selected.
---
*/
using System.Xml.Serialization;

namespace AssemblyCSharp.LevelData
{
    public class CampaignLevel
    {
        [XmlAttribute("FileName")]
        public string FileName { get; set; }

        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; }
    }
}
