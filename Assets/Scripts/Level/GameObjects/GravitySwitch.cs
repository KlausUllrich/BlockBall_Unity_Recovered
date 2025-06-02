using UnityEngine;
using System.Xml;
using System.Collections;

public class GravitySwitch : Block//GrafikObjekt 
{
    //-----------------------------------------------------------------------------------------------------------------
    public static new GravitySwitch Create(GameObject pParent, XmlElement pElement)
    {		
		var sPrefab = XmlUtils.GetXmlAttributeString(pElement, "type");
		var pObject = GrafikObjekt.Create(pParent, "GravitySwitch/" + sPrefab) as GravitySwitch;
		BlockBall.Debug.ASSERT(pObject != null);
		pObject.InitializeByXml(pElement);

        return pObject;
	}
    
	//-----------------------------------------------------------------------------------------------------------------
}
