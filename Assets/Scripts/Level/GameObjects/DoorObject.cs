using UnityEngine;
using System.Collections;
using System.Xml;

public class DoorObject : EffectObject {
	
	public int iId { get; private set; }
	public bool bClose { get; private set; }
	public GameObject xLeaf;
	public GameObject xJamb;
	//-----------------------------------------------------------------------------------------------------------------
	protected Color xColor;
	
	//-----------------------------------------------------------------------------------------------------------------
    public static new DoorObject Create(GameObject pParent, XmlElement pElement)
    {		
		var pObject = DoorObject.Create(pParent, "Door") as DoorObject;
		BlockBall.Debug.ASSERT(pObject != null);
		pObject.InitializeByXml(pElement);
		
        return pObject;
	}
	
	//-----------------------------------------------------------------------------------------------------------------
	protected override void InitializeByXml(XmlElement pElement)
    {
        base.InitializeByXml(pElement);		
		
        iId = XmlUtils.GetXmlAttributeInt(pElement, "KeyId", -1);
		bClose = iId != -1;
	
		var vColor = XmlUtils.GetXmlAttributeVector3(pElement, "color", Vector3.one);
		this.SetColor(new Color(vColor.x, vColor.y, vColor.z, 1));
	}   

	//-----------------------------------------------------------------------------------------------------------------
	public void SetColor(Color color)
	{
		this.xColor = color;
		
		if (xLeaf.GetComponent<Renderer>() != null)
		{
			xLeaf.GetComponent<Renderer>().material.color = this.xColor;
		}
		if (xJamb.GetComponent<Renderer>() != null)
		{
			xJamb.GetComponent<Renderer>().material.color = this.xColor;
		}
	}
	
	//-----------------------------------------------------------------------------------------------------------------
    public void Open()
    {
		GameObject.Destroy(xLeaf.gameObject);
	}
	
	//-----------------------------------------------------------------------------------------------------------------
}
