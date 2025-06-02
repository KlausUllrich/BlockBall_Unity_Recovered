using UnityEngine;
using System.Xml;
using System.Collections;

public class DoorHelper : MonoBehaviour
{
    //-----------------------------------------------------------------------------------------------------------------
    public DoorObject Door;

    //-----------------------------------------------------------------------------------------------------------------
    void Start()
    {
    }

    //-----------------------------------------------------------------------------------------------------------------
    void OnTriggerEnter(Collider pCollider)
	{
		if (Door.bClose == false)
			return;

        var pPlayer = pCollider.gameObject.GetComponent<Player>();
		BlockBall.Debug.ASSERT(pPlayer);
        if (pPlayer != null)
        {
			if (pPlayer.aiKeys.Contains(Door.iId))
				Door.Open();
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
}
