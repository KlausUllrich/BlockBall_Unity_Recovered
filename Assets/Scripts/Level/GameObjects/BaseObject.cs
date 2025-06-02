using UnityEngine;
using System.Xml;
using System;
using System.Collections;

public class BaseObject : MonoBehaviour
{
    //-----------------------------------------------------------------------------------------------------------------
    protected static readonly string PrefabFolder = "Prefabs/";
    protected static readonly string MissingPrefab = PrefabFolder + "MissingPrefab";

    //-----------------------------------------------------------------------------------------------------------------
    protected string PrefabName { get; private set; }
	
    //-----------------------------------------------------------------------------------------------------------------
    public static BaseObject Create(string sPrefab)
    {
        if (String.IsNullOrEmpty(sPrefab))
            sPrefab = "BaseObject";

        UnityEngine.Object prefab = Resources.Load(PrefabFolder + sPrefab);
        bool bInvalid = (prefab == null);
        if (bInvalid)
            prefab = Resources.Load(MissingPrefab);

        var pGO = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        if (bInvalid)
        {
            Debug.LogWarning("Prefab '" + sPrefab + "' not found use MissingPrefab instead!", pGO);
        }

        var pObject = pGO.GetComponent<BaseObject>();
        if (pObject != null)
        {
            pObject.PrefabName = sPrefab;
        }
        else
        {
            Debug.LogError("Prefab '" + sPrefab + "' has missing BaseObject script! Removing it now !!");
            GameObject.DestroyImmediate(pGO);
        }

        return pObject;
    }

    //-----------------------------------------------------------------------------------------------------------------
    public static BaseObject Create(GameObject pParent, string sPrefab)
    {
        var pObject = Create(sPrefab);
        if (pObject != null)
        {
            if (pParent != null)
                pObject.transform.parent = pParent.transform;
        }
        return pObject;
    }
	
    //-----------------------------------------------------------------------------------------------------------------
    public static BaseObject Create(GameObject pParent, XmlElement pElement)
    {		
		var pObject = BaseObject.Create(pParent, "");
		BlockBall.Debug.ASSERT(pObject != null);
		pObject.InitializeByXml(pElement);

        return pObject;
    }
		
    //-----------------------------------------------------------------------------------------------------------------
    public static BaseObject Create(GameObject pParent, string sPrefab, Vector3 vPosition, Quaternion qRotation)
    {
        var pObject = Create(sPrefab);
        if (pObject != null)
        {
            pObject.transform.position = vPosition;
            pObject.transform.rotation = qRotation;

            if (pParent != null)
                pObject.transform.parent = pParent.transform;
        }
        return pObject;
    }
    
    //-----------------------------------------------------------------------------------------------------------------
    protected virtual void InitializeByXml(XmlElement pElement)
    {
        this.transform.position = XmlUtils.GetXmlAttributeVector3(pElement, "pos", Vector3.zero); ;
        this.transform.rotation = XmlUtils.GetXmlAttributeQuaternion(pElement, "ori", Quaternion.identity);
    }

    //-----------------------------------------------------------------------------------------------------------------
    public virtual void InitializePostProcess()
    {
    }

    //-----------------------------------------------------------------------------------------------------------------
}
