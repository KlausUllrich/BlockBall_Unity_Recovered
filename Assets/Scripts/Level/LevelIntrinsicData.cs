/*
# ---
# Intent: Defines the data structure for the <MetaData> section within .level XML files.
#         This includes information like the level's internal name, creator, texture set, skybox, music track,
#         time tiers for medals, and a thumbnail image path.
# Architecture:
#   - A C# class designed for XML serialization and deserialization using System.Xml.Serialization attributes.
#   - Instantiated and populated by Level.cs when a .level file is parsed.
#   - Contains properties that map directly to XML elements within the <MetaData> tag.
# Relations:
#   - LevelIntrinsicData.cs: This file.
#   - Level.cs: Instantiates and uses this class to hold the parsed metadata from a .level file.
#   - TimeTierData.cs: Defines the structure for the <TimeTiers> element, which is a property of this class.
#   - .level files: This class defines the expected XML structure for the <MetaData> section in these files.
# ---
*/
using System.Xml.Serialization;

namespace AssemblyCSharp.LevelData
{
    [XmlRoot("MetaData")]
    public class LevelIntrinsicData
    {
        [XmlElement("LevelName")]
        public string LevelName { get; set; }

        [XmlElement("Creator")]
        public string Creator { get; set; }

        [XmlElement("TextureSet")]
        public string TextureSet { get; set; }

        [XmlElement("Skybox")]
        public string Skybox { get; set; }

        [XmlElement("Music")]
        public string Music { get; set; }

        // This will look for <TimeTiers MadGenius="..." ... />
        [XmlElement("TimeTiers")]
        public TimeTierData TimeTiers { get; set; }

        [XmlElement("Thumbnail")]
        public string Thumbnail { get; set; }

        public LevelIntrinsicData()
        {
            // Initialize with default values if necessary
            TimeTiers = new TimeTierData(); 
        }
    }
}
