using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BlockBall;
using UnityEngine.UI; // Added for Text component
using TMPro; // Added for TextMeshPro support

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Player : BallObject 
{
	// -------------------------------------------------------------------------------------------
	// -------------------------------------------------------------------------------------------
	public int iScore { get; private set; }
	public List<int> aiKeys { get; private set; }
	public DeadZoneCollider pDeadZoneCollider;

	// -------------------------------------------------------------------------------------------
	private Vector3 xLastCollectedScoreItemPosition;
	private Quaternion xOrientationAsLastCollectedScoreItem;
    private Vector3 xLastCollectedScoreItemGravityDirection;
    private Vector3 xLastCollectedScoreItemForwardDirection;
    private PhysicObjekt pPlayerPhysicObject;

	// ---------- TEST ---------------------------------------------------------------------------
	public GameObject pScoreText;
	public GameObject pKeysText;
	// ---------- TEST END -----------------------------------------------------------------------
	
	// -------------------------------------------------------------------------------------------


    //-----------------------------------------------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        pPlayerPhysicObject = this.GetComponent<PhysicObjekt>();
        // Replace deprecated sleepVelocity with sleepThreshold
        this.GetComponent<Rigidbody>().sleepThreshold = 0.0f;
    }

    // -------------------------------------------------------------------------------------------
	void Start () 
	{
		this.iScore = 0;
		this.aiKeys = new List<int>();			
		this.xLastCollectedScoreItemPosition = this.transform.position;
		this.xOrientationAsLastCollectedScoreItem = this.transform.localRotation;
        this.xLastCollectedScoreItemGravityDirection = this.GravityDirection;
        this.xLastCollectedScoreItemForwardDirection = this.ForwardDirection;
	}

    //-----------------------------------------------------------------------------------------------------------------
    public override void SetGravityDirection(Vector3 vDirection)
    {
        base.SetGravityDirection(vDirection);
    }

	// -------------------------------------------------------------------------------------------
	void OnTriggerEnter (Collider pCollider)
	{
		var pBaseObject = pCollider.gameObject.GetComponent(typeof(BaseObject));
				
		if (pBaseObject == null)
		{
			if(pCollider.gameObject == pDeadZoneCollider.gameObject)
				ResetPosition();
			else
				return;
		}
		
		if (pBaseObject is ScoreItem)
		{
			var scoreItem = pBaseObject as ScoreItem;
			iScore += scoreItem.Score;
			
			// Update score text using modern UI components - but only if pScoreText is assigned
			if (pScoreText != null)
			{
				TextMeshProUGUI tmpText = pScoreText.GetComponent<TextMeshProUGUI>();
				if (tmpText != null)
				{
					tmpText.text = "Score: " + iScore.ToString();
				}
				else
				{
					Text legacyText = pScoreText.GetComponent<Text>();
					if (legacyText != null)
					{
						legacyText.text = "Score: " + iScore.ToString();
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning("pScoreText is not assigned - score UI will not update");
			}
			
			SaveLastCollectedScoreItemPosition(scoreItem);
		}
		else if (pBaseObject is KeyItem)
		{
			var keyItem = pBaseObject as KeyItem;
			aiKeys.Add(keyItem.iId);
			PrintKeys();
		}
		else if (pBaseObject is DoorObject)
		{
			// ToDo: nur wenn Door noch zu ist, bzw. HelperObjekt noch da ists
			var doorObject = pBaseObject as DoorObject;
			aiKeys.Remove(doorObject.iId);
			PrintKeys();
		}
	}

	// -------------------------------------------------------------------------------------------
	void PrintKeys ()
	{
		// Create the keys string
		string keysString = "Keys: ";
		foreach(var iKey in aiKeys)
		{
			keysString += iKey + ", ";		
		}
		keysString = keysString.TrimEnd(' ').TrimEnd(',');
		
		// Update text using modern UI components
		TextMeshProUGUI tmpText = pKeysText.GetComponent<TextMeshProUGUI>();
		if (tmpText != null)
		{
			tmpText.text = keysString;
		}
		else
		{
			Text legacyText = pKeysText.GetComponent<Text>();
			if (legacyText != null)
			{
				legacyText.text = keysString;
			}
		}
	}
	
	// -------------------------------------------------------------------------------------------
	void SaveLastCollectedScoreItemPosition (ScoreItem xScoreItem)
	{
		// Save the PLAYER's current position and orientation when collecting the diamond
		// NOT the diamond's position - the player should respawn where they were, not where the diamond was
		this.xLastCollectedScoreItemPosition			= this.transform.position;
        this.xOrientationAsLastCollectedScoreItem		= this.transform.localRotation;
        this.xLastCollectedScoreItemGravityDirection	= this.GravityDirection;
        this.xLastCollectedScoreItemForwardDirection	= this.ForwardDirection;
        
        UnityEngine.Debug.Log($"Saved respawn position: pos={this.transform.position}, rot={this.transform.localRotation}");
	}
	
	// -------------------------------------------------------------------------------------------
	public void ResetPosition ()
	{
		UnityEngine.Debug.Log($"Resetting to saved position: pos={this.xLastCollectedScoreItemPosition}, rot={this.xOrientationAsLastCollectedScoreItem}");
		
		this.transform.position			= this.xLastCollectedScoreItemPosition;
		this.transform.localRotation	= this.xOrientationAsLastCollectedScoreItem;
		this.GetComponent<Rigidbody>().linearVelocity			= Vector3.zero;
        this.SetGravityDirection(this.xLastCollectedScoreItemGravityDirection);
        this.SetForwardDirection(this.xLastCollectedScoreItemForwardDirection);
        
        // Also reset the camera to follow the player properly
        // The camera should automatically follow the player's new position and orientation
        UnityEngine.Debug.Log($"Reset complete. Player gravity: {this.GravityDirection}, forward: {this.ForwardDirection}");
    }

    // -------------------------------------------------------------------------------------------
    protected override void OnGroundContact(GameObject xGo, bool bValue)
    {
		base.OnGroundContact(xGo, bValue);

		if (bValue)
        {
            this.SetDeadZoneCollider();
        }
    }
	
	// -------------------------------------------------------------------------------------------
	private void SetDeadZoneCollider ()
	{
		pDeadZoneCollider.SetPositionInRelationToPlayer(this);
	}

    // -------------------------------------------------------------------------------------------
    void Update()
    {
        // Fall height death system - reset if player falls too far below last safe position
        float fallDistance = Vector3.Distance(this.transform.position, this.xLastCollectedScoreItemPosition);
        if (fallDistance > 20.0f) // 20 units is a reasonable fall distance
        {
            UnityEngine.Debug.Log($"Player fell too far (distance: {fallDistance:F1}), resetting position");
            ResetPosition();
        }
    }
	
	// -------------------------------------------------------------------------------------------
	
}
