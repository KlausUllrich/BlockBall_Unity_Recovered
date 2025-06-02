using UnityEngine;
using System.Collections;
using System.Xml;
using TMPro; // Added for TextMeshPro support
using UnityEngine.UI; // Added for Text component

public class InfoObject : EffectObject {
	public string Text { get; private set; }
	protected GameObject pGUILable;
		
	// -------------------------------------------------------------------------------------------
	protected new void Start () 
	{		
		base.Start();
		
		this.pGUILable = GameObject.Find("DevPlayerInfoLable");
		BlockBall.Debug.ASSERT(this.pGUILable);
	}
	
	//-----------------------------------------------------------------------------------------------------------------
    public static new InfoObject Create(GameObject pParent, XmlElement pElement)
    {		
		var pObject = GrafikObjekt.Create(pParent, "Info") as InfoObject;
		BlockBall.Debug.ASSERT(pObject != null);
		pObject.InitializeByXml(pElement);

        return pObject;
	}
	
	//-----------------------------------------------------------------------------------------------------------------
	protected override void InitializeByXml(XmlElement pElement)
    {
        base.InitializeByXml(pElement);		
		
        Text = XmlUtils.GetXmlAttributeString(pElement, "Text");
    }
	
	// -------------------------------------------------------------------------------------------
	void OnTriggerEnter(Collider pCollider)
	{
		if (pCollider.gameObject.tag == "Player")
		{
			// Update to use Text or TextMeshPro component instead of deprecated guiText
			TextMeshProUGUI textComponent = pGUILable.GetComponent<TextMeshProUGUI>();
			if (textComponent != null)
			{
				textComponent.text = Text;
			}
			else
			{
				// Fallback to legacy Text component if TextMeshPro is not found
				Text textLegacy = pGUILable.GetComponent<Text>();
				if (textLegacy != null)
				{
					textLegacy.text = Text;
				}
			}
		}
	}
	
	// -------------------------------------------------------------------------------------------
	void OnTriggerExit(Collider pCollider)
	{
		if (pCollider.gameObject.tag == "Player")
		{
			// Update to use Text or TextMeshPro component instead of deprecated guiText
			TextMeshProUGUI textComponent = pGUILable.GetComponent<TextMeshProUGUI>();
			if (textComponent != null)
			{
				textComponent.text = string.Empty;
			}
			else
			{
				// Fallback to legacy Text component if TextMeshPro is not found
				Text textLegacy = pGUILable.GetComponent<Text>();
				if (textLegacy != null)
				{
					textLegacy.text = string.Empty;
				}
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------------------
}
