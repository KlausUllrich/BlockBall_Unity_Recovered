using UnityEngine;
using System.Collections;

public class ConsumableObject : InteractObject {
		
	// -------------------------------------------------------------------------------------------
	void OnTriggerEnter(Collider pCollider)
	{
		if (pCollider.gameObject.tag == "Player")
		{
			GameObject.Destroy(this.gameObject);
		}
	}
	
	//-----------------------------------------------------------------------------------------------------------------
}
