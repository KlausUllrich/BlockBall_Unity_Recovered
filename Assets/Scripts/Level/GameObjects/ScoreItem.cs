using UnityEngine;
using System.Collections;
using System.Xml;

public class ScoreItem : ConsumableObject {
	
	public int Score { get; private set; }
    
	//-----------------------------------------------------------------------------------------------------------------
    public static new ScoreItem Create(GameObject pParent, XmlElement pElement)
    {		
		var pObject = GrafikObjekt.Create(pParent, "Diamond") as ScoreItem;
		BlockBall.Debug.ASSERT(pObject != null);
		pObject.InitializeByXml(pElement);
		
		pObject.transform.localScale = new Vector3(1 + 0.1f *pObject.Score, 1 + 0.1f *pObject.Score, 1 + 0.1f *pObject.Score);
		
        return pObject;
	}       
	
	//-----------------------------------------------------------------------------------------------------------------
	protected override void InitializeByXml(XmlElement pElement)
    {
        base.InitializeByXml(pElement);		
		
        Score = XmlUtils.GetXmlAttributeInt(pElement, "value", -1);
		BlockBall.Debug.ASSERT(Score != -1);
    }
	
	//-----------------------------------------------------------------------------------------------------------------
}
