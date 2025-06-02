using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class Campain
	{
		public List<LevelSet> LevelSets; 
		
		public Campain(string levelSetListFilePath)
		{
			LevelSets = new List<LevelSet>();
			
			// load campain file
			XmlDocument levelSetListDoc = new XmlDocument();
			levelSetListDoc.Load(levelSetListFilePath);
	
			// load every level set file from the campain file
			XmlNode levelSetListRoot = levelSetListDoc.FirstChild;
			foreach (XmlNode levelSetNode in levelSetListRoot.ChildNodes)
			{			
				var levelSet = new LevelSet();
				levelSet.Name = levelSetNode.Attributes["Name"].InnerText;
				levelSet.Levels = new List<Level>();
				
				XmlDocument levelSetDoc = new XmlDocument();
				var levelSetFileName = Definitions.relLevelFolder + levelSet.Name.Replace(" ", "_") + "." + Definitions.LevelSetFileExtention;
				levelSetDoc.Load(levelSetFileName);
				XmlNode levelSetRoot = levelSetDoc.FirstChild;
				
				// load every level from the set
				// !Only the level info will be load. The real Level can creat with Create()!
				foreach(XmlNode levelNode in levelSetRoot.ChildNodes)
				{
					var levelFileName = levelNode.Attributes["Name"].InnerText.Replace(" ", "_") + Definitions.CampainLevelPostFix;
					var level = new Level(levelFileName);
					
					// add level to level set
					levelSet.Levels.Add(level);
				}
				
				// add levelset to campain
				LevelSets.Add(levelSet);
			}
		}
	}
}

