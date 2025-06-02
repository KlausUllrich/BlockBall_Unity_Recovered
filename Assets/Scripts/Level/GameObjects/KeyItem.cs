using UnityEngine;
using System.Collections;
using System.Xml;

public class KeyItem : ConsumableObject {
		
	public int iId { get; private set; }
	//-----------------------------------------------------------------------------------------------------------------
	protected Color xColor;

	//-----------------------------------------------------------------------------------------------------------------
	public new void Start()
	{
		GetComponent<Renderer>().material.color = xColor;
	}

	//-----------------------------------------------------------------------------------------------------------------
    public static new KeyItem Create(GameObject pParent, XmlElement pElement)
    {		
		var pObject = GrafikObjekt.Create(pParent, "Key") as KeyItem;
		BlockBall.Debug.ASSERT(pObject != null);
		pObject.InitializeByXml(pElement);

        return pObject;
	}    
	
	//-----------------------------------------------------------------------------------------------------------------
	protected override void InitializeByXml(XmlElement pElement)
    {
        base.InitializeByXml(pElement);		
		
        iId = XmlUtils.GetXmlAttributeInt(pElement, "KeyId", -1);
		BlockBall.Debug.ASSERT(iId != -1);
				
		var vColor = XmlUtils.GetXmlAttributeVector3(pElement, "color", Vector3.one);
		this.SetColor(new Color(vColor.x, vColor.y, vColor.z, 1));
	}   
	
	//-----------------------------------------------------------------------------------------------------------------
	public void SetColor(Color color)
	{
		this.xColor = color;
		
		if (this.GetComponent<Renderer>() != null)
		{
			this.GetComponent<Renderer>().material.color = this.xColor;
		}
	}
		
	//-----------------------------------------------------------------------------------------------------------------
}
