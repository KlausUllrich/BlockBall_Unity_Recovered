using UnityEngine;
using System.Collections;
using System.Xml;

public class Goal : EffectObject {
	//-----------------------------------------------------------------------------------------------------------------
    public static new Goal Create(GameObject pParent, XmlElement pElement)
    {		
		var pObject = GrafikObjekt.Create(pParent, "Goal") as Goal;
		BlockBall.Debug.ASSERT(pObject != null);
		pObject.InitializeByXml(pElement);

        return pObject;
	}
	
	//-----------------------------------------------------------------------------------------------------------------
}
