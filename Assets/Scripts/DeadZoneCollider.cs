using UnityEngine;
using System.Collections;

public class DeadZoneCollider : MonoBehaviour 
{
	// -------------------------------------------------------------------------------------------
	public void SetPositionInRelationToPlayer(Player pPlayer) 
	{
        this.transform.rotation = Quaternion.FromToRotation(Vector3.up, -pPlayer.GravityDirection);
		this.transform.position = pPlayer.transform.position + pPlayer.GravityDirection * 4;
	}
	
	// -------------------------------------------------------------------------------------------
	void Update () 
	{
	
	}	
	// -------------------------------------------------------------------------------------------
}
