/*
--- YAML HEADER ---
Intent: Represents the root structure of the game's campaign, containing a collection of level sets.
  This class is the top-level object deserialized from 'campaign_definition.xml'.
Architecture: This class is a data container designed for XML serialization.
  - It primarily holds a list of 'CampaignLevelSet' objects.
  - The [XmlRoot("CampaignDefinition")] attribute specifies the root XML element name.
  - The [XmlElement("LevelSet")] attribute maps to the <LevelSet> elements in the XML.
Relations:
  - Is the root object for: 'campaign_definition.xml' deserialization.
  - Contains: A List<CampaignLevelSet> (CampaignLevelSet.cs objects).
  - Managed by: CampaignManager.cs (which loads and stores an instance of this class).
---
*/
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AssemblyCSharp.LevelData
{
    [XmlRoot("CampaignDefinition")]
    public class CampaignDefinition
    {
        [XmlElement("LevelSet")]
        public List<CampaignLevelSet> LevelSets { get; set; }

        public CampaignDefinition()
        {
            LevelSets = new List<CampaignLevelSet>();
        }
    }
}
