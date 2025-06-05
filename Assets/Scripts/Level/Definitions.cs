using UnityEngine;
using System.IO;
using System.Collections;

public class Definitions
{
    public static readonly string ResourcesLevelsSubFolderPath = "Levels";
	public static DirectoryInfo relLevelFolder = new DirectoryInfo("Assets/Resources/" + ResourcesLevelsSubFolderPath + "/");
	public static string CampainLevelPostFix = "_campain";
	public static string LevelFileExtention = "level";
	public static string LevelInfoFileExtention = "levelinfo";
	public static string LevelSetFileExtention = "level_set";
	public static string LevelSetListFileExtention = "level_set_list";

	public static float BallTouchingTheGroundThresholdAsDotProductResult = 0.866f; //> 30°
}
