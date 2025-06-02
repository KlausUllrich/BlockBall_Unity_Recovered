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
			
			// Update score text using modern UI components
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
		this.xLastCollectedScoreItemPosition			= xScoreItem.transform.position;
        this.xOrientationAsLastCollectedScoreItem		= xScoreItem.transform.localRotation; //this.transform.localRotation;
        this.xLastCollectedScoreItemGravityDirection	= (xScoreItem.transform.rotation * Vector3.down).normalized; //this.GravityDirection;
        this.xLastCollectedScoreItemForwardDirection	= (xScoreItem.transform.rotation * Vector3.forward).normalized; //this.ForwardDirection;
	}
	
	// -------------------------------------------------------------------------------------------
	public void ResetPosition ()
	{
		this.transform.position			= this.xLastCollectedScoreItemPosition;
		this.transform.localRotation	= this.xOrientationAsLastCollectedScoreItem;
		this.GetComponent<Rigidbody>().velocity			= Vector3.zero;
        this.SetGravityDirection(this.xLastCollectedScoreItemGravityDirection);
        this.SetForwardDirection(this.xLastCollectedScoreItemForwardDirection);
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
	
}
