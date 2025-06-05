/*
# ---
# Intent: Represents a single game level, managing its definition, game objects, and metadata.
# Architecture:
#   - Loaded and managed by the game's level loading system (e.g., BlockMerger, LevelSelectionManager).
#   - Parses .level XML files to instantiate game objects (Blocks, StartObject, Goal, etc.) and metadata.
#   - Holds intrinsic level data (LevelIntrinsicData) and a collection of all BaseObjects within the level.
# Relations:
#   - Level.cs: This file.
#   - LevelIntrinsicData.cs: Defines the structure for the <MetaData> section in .level files.
#   - BaseObject.cs (and its derivatives like Block.cs, StartObject.cs, Goal.cs): Represents individual game elements within the level.
#   - Definitions.cs: Provides constants like level file paths and extensions.
#   - XmlUtils.cs: Utility functions for XML parsing.
#   - CampaignManager.cs: While not directly used by Level.cs for loading itself, CampaignManager uses level filenames which Level.cs then loads.
#   - Level files (.level): XML files that describe the level structure and content.
# ---
*/
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO; // For StringReader
using AssemblyCSharp.LevelData; // For our new data classes

namespace AssemblyCSharp
{
    public class Level
    {
        // -------------------------------------------------------------------------------------------
        public class PositionHashEntry : System.Object
        {
            public int X { get; private set;}
            public int Y { get; private set;}
            public int Z { get; private set;}

            // -------------------------------------------------------------------------------------------
            private int Hashcode;

            // -------------------------------------------------------------------------------------------
            public PositionHashEntry(Vector3 vPos)
            {
                this.X = Mathf.RoundToInt(vPos.x);
                this.Y = Mathf.RoundToInt(vPos.y);
                this.Z = Mathf.RoundToInt(vPos.z);
                this.CalcHashcode();
            }

            // -------------------------------------------------------------------------------------------
            public PositionHashEntry(int x, int y, int z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.CalcHashcode();
            }

            // -------------------------------------------------------------------------------------------
            private void CalcHashcode()
            {
                // 2^11 = 2048 (x,y) 2^10 = 1024 (z) => 31-21 = x, 20-10 = y, 9 - 0 = z;
                this.Hashcode = 0;
                this.Hashcode += (this.X % 2048) << 21;     // 11bit (21-31)
                this.Hashcode += (this.Y % 2048) << 10;     // 11bit (10-20)
                this.Hashcode += (this.Z % 1024);           // 10bit (0-9)

                //this.Hashcode = this.X / 3 + this.Y / 3 + this.Z / 3;
            }
            
            // -------------------------------------------------------------------------------------------
            public override bool Equals(System.Object obj)
            {
                var pOther = obj as PositionHashEntry;
                return (pOther.X == this.X) && (pOther.Y == this.Y) && (pOther.Z == this.Z);
            }

            // -------------------------------------------------------------------------------------------
            public override int GetHashCode()
            {
                return this.Hashcode;
            }
            
            // -------------------------------------------------------------------------------------------
        }

        // -------------------------------------------------------------------------------------------
        public string FileName;
        public LevelInfo LevelInfo; // Will likely be deprecated by IntrinsicData
        public LevelIntrinsicData IntrinsicData { get; private set; }

        public Dictionary<PositionHashEntry, Block> PositionHash = new Dictionary<PositionHashEntry, Block>();
        public List<List<Block>> GroupList = new List<List<Block>>();

        // -------------------------------------------------------------------------------------------
        private List<BaseObject> LevelObjectList = new List<BaseObject>();

        // -------------------------------------------------------------------------------------------
        public Level(string fileName)
        {
            FileName = fileName;
            // Noch haben wir keine wirkliche LevelInfodatei, aber wenn dann sie hier laden.
            LevelInfo = new LevelInfo{LevelName=fileName};
        }

        // -------------------------------------------------------------------------------------------
        public void Destroy()
        {
            GameObject.DestroyImmediate(GameObject.Find("LevelCubes"));
            GameObject.DestroyImmediate(GameObject.Find("BlockGroups"));
            
            // Clear all collections to prevent memory leaks and conflicts
            PositionHash.Clear();
            LevelObjectList.Clear();
            GroupList.Clear();
        }

        // -------------------------------------------------------------------------------------------
        public void Build()
        {
            // Clear previous level data to prevent conflicts when reloading
            PositionHash.Clear();
            LevelObjectList.Clear();
            GroupList.Clear();
            
            // Check if a level is loaded
            if (GameObject.Find("LevelCubes") != null)
                throw new Exception("A level is already loaded. You must destroy it before you can load a new one.");
                
            var levelRoot = new GameObject("LevelCubes");
            // ToDo: Testen ob es mit StreamReader auch geht
            // Path for Resources.Load should be relative to any Resources folder,
            // e.g., "Levels/first_roll_campain" if files are in Assets/Resources/Levels/
            // FileName (from Level.FileName) does not include the .level extension.
            // Construct the direct file path. Definitions.relLevelFolder.ToString() gives "Assets/Resources/Levels/"
            string levelFilePath = Definitions.relLevelFolder.ToString() + FileName + "." + Definitions.LevelFileExtention;
            
            // For robust logging, let's also see the absolute path this resolves to
            string absolutePathForLog = "";
            try 
            {
                absolutePathForLog = System.IO.Path.GetFullPath(levelFilePath);
            }
            catch 
            {
                absolutePathForLog = "Error resolving full path for: " + levelFilePath;
            }

            Debug.Log($"[Level.Build] FileName: '{FileName}'. Attempting to load XML directly from relative path: '{levelFilePath}', which should resolve to absolute path: '{absolutePathForLog}'");
            
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(levelFilePath); // Directly load from the file system path
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.LogError($"Direct XML Load Error: File not found at path: '{levelFilePath}' (resolved to '{absolutePathForLog}'). Original Exception: {ex.Message}");
                throw new System.IO.FileNotFoundException($"Level file not found at path: {levelFilePath}. Ensure the file exists at this location relative to the project root.", levelFilePath, ex);
            }
            catch (System.Xml.XmlException ex)
            {
                 Debug.LogError($"Direct XML Load Error: XML parsing error from file: '{levelFilePath}' (resolved to '{absolutePathForLog}'). Original Exception: {ex.Message}");
                 throw new System.Xml.XmlException($"Level file XML error: {levelFilePath}. {ex.Message}", ex);
            }
            catch (System.Exception ex) // Catch any other potential exceptions during load
            {
                Debug.LogError($"Direct XML Load Error: An unexpected error occurred while loading file: '{levelFilePath}' (resolved to '{absolutePathForLog}'). Original Exception: {ex.Message}");
                throw; 
            } // Load XML from the TextAsset's content

            // Get the root element (should be <BlockBallLevel>)
            XmlElement blockBallLevelNode = xmlDoc.DocumentElement;
            if (blockBallLevelNode == null || blockBallLevelNode.Name != "BlockBallLevel")
            {
                throw new XmlException("Expected <BlockBallLevel> root node in level file: " + FileName);
            }

            // Parse MetaData
            XmlNode metaDataNode = blockBallLevelNode.SelectSingleNode("MetaData");
            if (metaDataNode != null)
            {
                XmlSerializer metaSerializer = new XmlSerializer(typeof(LevelIntrinsicData));
                using (StringReader reader = new StringReader(metaDataNode.OuterXml))
                {
                    this.IntrinsicData = (LevelIntrinsicData)metaSerializer.Deserialize(reader);
                }
            }
            else
            {
                Debug.LogWarning("<MetaData> section not found in level: " + FileName + ". Using default intrinsic data.");
                this.IntrinsicData = new LevelIntrinsicData(); // Initialize with defaults if not found
            }

            // Get the BB3Level node for game object creation
            XmlNode bb3LevelNode = blockBallLevelNode.SelectSingleNode("BB3Level");
            if (bb3LevelNode == null)
            {
                throw new XmlException("<BB3Level> node not found within <BlockBallLevel> in level file: " + FileName);
            }
            
            // Create each Object from the BB3Level node
            // XmlNode root = xml.FirstChild; // Old: root was DocumentElement
            // Now, iterate over children of bb3LevelNode
            foreach (XmlNode pNode in bb3LevelNode.ChildNodes)
            {
                var pElement = pNode as XmlElement;
                if (pElement == null)
                    continue;
                
                BaseObject pObject = null;
				switch (pNode.Name)
				{
				case "StartObject":
	                {
	                    var pos = XmlUtils.GetXmlAttributeVector3(pElement, "pos", Vector3.zero);
	                    var ori = XmlUtils.GetXmlAttributeQuaternion(pElement, "ori", Quaternion.identity);
	                    Debug.Log($"StartObject: pos={pos}, ori={ori}");
	                    var t = GameObject.Find("PlayerSphere");
	                    if (t != null)
	                    {
	                        Debug.Log($"Found PlayerSphere, setting position to {pos}");
	                        t.transform.position = pos;
	                        t.transform.rotation = ori;

	                        var pPlayer = t.GetComponent<Player>();
	                        if (pPlayer != null)
	                        {
	                            pPlayer.SetGravityDirection(-t.transform.up);
	                            pPlayer.SetForwardDirection(ori * -Vector3.forward);
	                            
	                            // CRITICAL FIX: Update the saved position so ResetPosition() works correctly
	                            // BUT ONLY if no score items have been collected yet (preserve diamond respawn mechanic)
	                            // The Player.Start() method captures the initial position before StartObject is processed
	                            var lastPosField = typeof(Player).GetField("xLastCollectedScoreItemPosition", 
	                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
	                            var lastRotField = typeof(Player).GetField("xOrientationAsLastCollectedScoreItem", 
	                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
	                            
	                            if (lastPosField != null && lastRotField != null)
	                            {
	                                // iScore is a property, not a field - check it directly
	                                int currentScore = pPlayer.iScore;
	                                // Only update saved position if no diamonds collected yet (score == 0)
	                                if (currentScore == 0)
	                                {
	                                    lastPosField.SetValue(pPlayer, pos);
	                                    lastRotField.SetValue(pPlayer, t.transform.localRotation);
	                                    Debug.Log($"Updated Player initial saved position to {pos} (score=0, no diamonds collected)");
	                                }
	                                else
	                                {
	                                    Debug.Log($"Preserving diamond respawn position (score={currentScore}, diamonds collected)");
	                                }
	                            }
	                            else
	                            {
	                                Debug.LogWarning("Could not find Player position fields via reflection");
	                            }
	                        }
	                        else
	                        {
	                            Debug.LogWarning("PlayerSphere found but no Player component!");
	                        }
	                    }
	                    else
	                    {
	                        Debug.LogError("PlayerSphere GameObject not found!");
	                    }
						break;
	                }
				case "Block":
	                {
	                    pObject = Block.Create(levelRoot, pElement);
						break;
	                }
				case "BlockGS":
	                {
	                    pObject = GravitySwitch.Create(levelRoot, pElement);
						break;
	                }
				case "ScoreItem":
	                {
	                    pObject = ScoreItem.Create(levelRoot, pElement);
						break;
	                }
				case "GoalObject":
	                {
	                    pObject = Goal.Create(levelRoot, pElement);
						break;
	                }
				case "DoorObject":
	                {
	                    pObject = DoorObject.Create(levelRoot, pElement);
						break;
	                }
				case "KeyItem":
	                {
	                    pObject = KeyItem.Create(levelRoot, pElement);
						break;
	                }
				case "EnviromentObject":
	                {
	                    // ToDo
	                    pObject = BaseObject.Create(levelRoot, pElement);
						break;
	                }
				case "InfoObject":
	                {
	                    pObject = InfoObject.Create(levelRoot, pElement);
						break;
	                }
				default:
	                {
	                    pObject = BaseObject.Create(levelRoot, pElement);
						UnityEngine.Debug.Log(String.Format("Unknown Object: {0}", pNode.Name));
						break;
	                }
				}

                if (pObject != null)
                {
                    LevelObjectList.Add(pObject);
                }
            }

            // posprocess, attaching groups and do scripting initialisations
            foreach (var pObject in this.LevelObjectList)
            {
                pObject.InitializePostProcess();
            }

            // find visual groups (TESTING for BlockMerger)
            var pWatch = new System.Diagnostics.Stopwatch();
            pWatch.Start();
            this.AddLevelObjectsToPositionHash();
            this.MoveBlocksIntoGroups();
            pWatch.Stop();

            Debug.Log("Merger needed: " + pWatch.ElapsedMilliseconds + "ms");
        }

        // -------------------------------------------------------------------------------------------
        void AddLevelObjectsToPositionHash()
        {
            foreach (var pObject in this.LevelObjectList)
            {
                var pBlock = pObject as Block;
                if (pBlock != null)
                {
                    var pHash = new PositionHashEntry(pBlock.gameObject.transform.position);
                    try
                    {
                        this.PositionHash.Add(pHash, pBlock);
                    }
                    catch (Exception e)
                    {
                        var pOther = this.PositionHash[pHash];
                        Debug.LogWarning(e.Message +"\n"+ e.StackTrace, pOther.gameObject);
                        GameObject.DestroyImmediate(pBlock.gameObject);
                    }
                }
            }


            // HACK: add neighboars to blocks
            foreach (var pObject in this.LevelObjectList)
            {
                var pBlock = pObject as Block;
                if (pBlock != null)
                {
                    for (int i = 0; i < 6; ++i)
                    {
                        var vNeighborPosition = pBlock.gameObject.transform.position + this.GetNeighborPosition(i);
                        var pHashNeighbor = new PositionHashEntry(vNeighborPosition);
                        Block pNeigbor = null;
                        this.PositionHash.TryGetValue(pHashNeighbor, out pNeigbor);
                        if (pNeigbor != null)
                            pBlock.AddNeighbor(pNeigbor);
                    }
                }
            }
        }

        // -------------------------------------------------------------------------------------------
        Vector3 GetNeighborPosition(int i)
        {
            switch (i)
            {
            case 0: return new Vector3(1, 0, 0);
            case 1: return new Vector3(-1, 0, 0);
            case 2: return new Vector3(0, 1, 0);
            case 3: return new Vector3(0, -1, 0);
            case 4: return new Vector3(0, 0, 1);
            case 5: return new Vector3(0, 0, -1);
            }

            return Vector3.zero;
        }

        // -------------------------------------------------------------------------------------------
        void MoveBlocksIntoGroups()
        {
            // find group
            var pOpenList = new HashSet<Block>();
            var pClosedList = new HashSet<Block>();
            foreach (var pStarterBlock in this.PositionHash.Values)
            {
                if (!pClosedList.Contains(pStarterBlock))
                {
                    List<Block> pGroup = new List<Block>();
                    pOpenList.Add(pStarterBlock);

                    while (pOpenList.Count > 0)
                    {
                        var pEnumerator = pOpenList.GetEnumerator();
                        pEnumerator.MoveNext();

                        var pBlock = pEnumerator.Current;
                        {
                            pClosedList.Add(pBlock);
                            pOpenList.Remove(pBlock);
                            pGroup.Add(pBlock);

                            this.AddNextBlocksToOpenList(pBlock, pOpenList, pClosedList);
                        }
                    }
                    this.GroupList.Add(pGroup);
                }
            }

            // grouping under gameobjects (just for debug test)
            var pGroupsObject = new GameObject("BlockGroups");
            int i = 0;
            foreach (var pGroup in this.GroupList)
            {
                var pGroupObject = new GameObject("BlockGroup" + i++);
                pGroupObject.transform.parent = pGroupsObject.transform;
                pGroupObject.transform.position = pGroup[0].gameObject.transform.position;
                foreach (var pBlock in pGroup)
                {
                    pBlock.gameObject.transform.parent = pGroupObject.transform;
                }
            }
        }

        // -------------------------------------------------------------------------------------------
        const float MergeMagnitude = 0.01f;
        void AddBlockToOpenList(Vector3 vPosition, HashSet<Block> pOpenList, HashSet<Block> pClosedList, MeshFilter pBlockMeshFilter)
        {
            Mesh pBlockMesh = pBlockMeshFilter.sharedMesh;
            Block pNextBlock;
            if (this.PositionHash.TryGetValue(new PositionHashEntry(vPosition), out pNextBlock))
            {
                if (!pOpenList.Contains(pNextBlock) && !pClosedList.Contains(pNextBlock))
                {
                    var pNextMesh = pNextBlock.MeshFilter.sharedMesh;
                    if (pNextMesh == null)
                        return;

                    var aNextVertices = pNextMesh.vertices;
                    var aBlockVertices = pBlockMesh.vertices;

                    for (int b = 0; b < pBlockMesh.vertexCount; ++b)
                    {
                        for (int n = 0; n < pNextMesh.vertexCount; ++n)
                        {
                            var vDist = pNextBlock.MeshFilter.transform.TransformPoint(aNextVertices[n]) - pBlockMeshFilter.transform.TransformPoint(aBlockVertices[b]);
                            if (vDist.sqrMagnitude <= MergeMagnitude)
                            {
                                pOpenList.Add(pNextBlock);
                                return;
                            }
                        }
                    }
                }
            }
        }

        // -------------------------------------------------------------------------------------------
        void AddNextBlocksToOpenList(Block pBlock, HashSet<Block> pOpenList, HashSet<Block> pClosedList)
        {
            var pMeshFilter = pBlock.MeshFilter;

            for (int i = 0; i < 3; ++i)
            {
                Vector3 vMid = Vector3.zero;
                switch (i)
                {
                case 0: vMid = pBlock.gameObject.transform.position + Vector3.up; break;
                case 1: vMid = pBlock.gameObject.transform.position; break;
                case 2: vMid = pBlock.gameObject.transform.position + Vector3.down; break;
                }

                if (i != 1)
                    this.AddBlockToOpenList(vMid, pOpenList, pClosedList, pMeshFilter);
                this.AddBlockToOpenList(vMid + Vector3.left, pOpenList, pClosedList, pMeshFilter);
                this.AddBlockToOpenList(vMid + Vector3.right, pOpenList, pClosedList, pMeshFilter);

                var vForward = vMid + Vector3.forward;
                this.AddBlockToOpenList(vForward, pOpenList, pClosedList, pMeshFilter);
                this.AddBlockToOpenList(vForward + Vector3.left, pOpenList, pClosedList, pMeshFilter);
                this.AddBlockToOpenList(vForward + Vector3.right, pOpenList, pClosedList, pMeshFilter);

                var vBackward = vMid + Vector3.back;
                this.AddBlockToOpenList(vBackward, pOpenList, pClosedList, pMeshFilter);
                this.AddBlockToOpenList(vBackward + Vector3.left, pOpenList, pClosedList, pMeshFilter);
                this.AddBlockToOpenList(vBackward + Vector3.right, pOpenList, pClosedList, pMeshFilter);
            }
        }

        // -------------------------------------------------------------------------------------------
    }
}
