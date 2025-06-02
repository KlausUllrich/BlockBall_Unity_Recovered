using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

[XmlRoot("BB3_LevelInfo")]
public class LevelInfo {
/*	Geplante LevelInfoDatei
<BB3_LevelInfo>
    <LevelName>LevelName</LevelName>
    <Creator>4Minds</Creator>
    <TextureSetId="0" />
    <Thumbnail></Thumbnail>
    <SkyBox>bb_skybox_1</SkyBox>
    <Screenshot></Screenshot>
    <Difficulty>beginner</Difficulty>
    <Music>first_roll.ogg</Music>
    <Medal time="14" />
    <Medal time="30" />
    <Medal time="60" />
    <Medal time="180" />
    <LevelCenter posx="0.0000" posy="0.0000" posz="0.0000" />
    <LevelZoom zoom="1754.9929" />
    <LevelOrientation orix="0.0000" oriy="0.0000" oriz="0.0000" oriw="1.0000" />
</BB3_LevelInfo>
*/	
	
	//-----------------------------------------------------------------------------------------------------------------
	[XmlAttribute("LevelName")]
	public string LevelName = "new level";
	
	[XmlAttribute("Creator")]
	public string Creator = "4Minds";
	
	[XmlAttribute("TextureSetId")]
	public int TextureSetId = 0;
		
	public static LevelInfo Load(string fileName)
	{
	/*	var levelSetFilePath = relLevelFolder + "levelsets.xml";
		var serializer = new XmlSerializer(typeof(LevelSetsRoot));
		var stream = new FileStream(levelSetFilePath, FileMode.Open);
		var campain = serializer.Deserialize(stream) as LevelSetsRoot;
		stream.Close();
		*/
		return new LevelInfo();
	}
}
