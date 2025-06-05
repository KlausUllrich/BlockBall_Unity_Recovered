/*
--- YAML HEADER ---
Intent: Defines the data structure for time-based achievements or scoring tiers within a level.
Architecture: This struct is a simple data container, primarily designed for XML serialization.
  It holds the time values for different performance tiers (e.g., Mad Genius, Grand Master).
  It is intended to be part of the LevelIntrinsicData, which is serialized from the <MetaData>
  section of a .level XML file.
Relations:
  - Contained by: LevelIntrinsicData.cs (as a field/property)
  - Serialized from/to: .level files (specifically the <TimeTiers> attributes within <MetaData>)
  - Used by: Gameplay systems that evaluate player completion times against these tiers.
---
*/
using System.Xml.Serialization;

namespace AssemblyCSharp.LevelData
{
    // This struct can be used directly by LevelIntrinsicData
    // and for XML serialization if attributes are used.
    public struct TimeTierData
    {
        [XmlAttribute("Mad_Genius_Time")]
        public float MadGeniusTime { get; set; }

        [XmlAttribute("Grand_Master_Time")]
        public float GrandMasterTime { get; set; }

        [XmlAttribute("Journeyman_Time")]
        public float JourneymanTime { get; set; }

        [XmlAttribute("Apprentice_Time")]
        public float ApprenticeTime { get; set; }
    }
}
