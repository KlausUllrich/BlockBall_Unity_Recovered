using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class Block : GrafikObjekt 
{
    //-----------------------------------------------------------------------------------------------------------------
    const string SeeThroughMaterialPath = "Materials/SeeThrough/";
    const string SeeThroughMaterialPostfix = "_SeeThrough";

    //-----------------------------------------------------------------------------------------------------------------
    protected Color xOriginalColor;
    protected Material pNormalMaterial;
    protected Material pSeeThroughMaterial;
    
    public bool bSeeThroughActivated;
    public List<Block> NeighborList = new List<Block>();

    //-----------------------------------------------------------------------------------------------------------------
    public static new Block Create(GameObject pParent, XmlElement pElement)
    {		
		var sPrefab = XmlUtils.GetXmlAttributeString(pElement, "type");
		var pObject = GrafikObjekt.Create(pParent, "Block/" + sPrefab) as Block;
		BlockBall.Debug.ASSERT(pObject != null);
		pObject.InitializeByXml(pElement);

        return pObject;
    }

	//-----------------------------------------------------------------------------------------------------------------
	public virtual void OnDestroy()
    {
        if (this.GetComponent<Renderer>() != null)
            this.GetComponent<Renderer>().sharedMaterial = null;
        if (this.pNormalMaterial != null)
            Material.Destroy(this.pNormalMaterial);
        this.pNormalMaterial = null;

        if (this.pSeeThroughMaterial != null)
            Material.Destroy(this.pNormalMaterial);
        this.pSeeThroughMaterial = null;
    }

	//-----------------------------------------------------------------------------------------------------------------
	protected override void InitializeByXml(XmlElement pElement)
    {
        base.InitializeByXml(pElement);

        this.bSeeThroughActivated = false;
        if (this.GetComponent<Renderer>() != null && this.GetComponent<Renderer>().sharedMaterial != null) //this.renderer.sharedMaterial == null happens on missing object
        {
            this.pNormalMaterial = new Material(this.GetComponent<Renderer>().sharedMaterial);
            this.GetComponent<Renderer>().sharedMaterial = this.pNormalMaterial;

            var sSeeThroughMaterialPath = string.Concat(SeeThroughMaterialPath, this.pNormalMaterial.name, SeeThroughMaterialPostfix);
            var pSeeThroughMaterial = Resources.Load(sSeeThroughMaterialPath, typeof(Material)) as Material;
            BlockBall.Debug.ASSERT(pSeeThroughMaterial != null, "Seethrough Material not found: '" + sSeeThroughMaterialPath + "'");
            if (pSeeThroughMaterial != null)
                this.pSeeThroughMaterial = new Material(pSeeThroughMaterial);
        }

        var vColor = XmlUtils.GetXmlAttributeVector3(pElement, "color", Vector3.one);
        this.xOriginalColor = new Color(vColor.x, vColor.y, vColor.z, 1);
        this.SetColor(this.xOriginalColor);
    }

    //-----------------------------------------------------------------------------------------------------------------
    public void AddNeighbor(Block pBlock)
    {
        if (!this.NeighborList.Contains(pBlock))
            this.NeighborList.Add(pBlock);
    }

	//-----------------------------------------------------------------------------------------------------------------
    public void SetContact(bool bActivate)
    {
        if (bActivate)
        {
            this.AdjustColor(Color.red);
        }
        else
        {
            this.RevertColor();
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    public void SetSeeThrough(bool bActivate)
    {
        if (this.GetComponent<Renderer>() == null)
            return;

        //if (this.bSeeThroughActivated == bActivate)
        //    return;

        if (bActivate)
        {
            if (this.pSeeThroughMaterial != null)
            {
                this.GetComponent<Renderer>().sharedMaterial = this.pSeeThroughMaterial;
                //this.renderer.sharedMaterial.SetFloat("_Alpha", 0.5f);
            }
        }
        else
        {
            this.GetComponent<Renderer>().sharedMaterial = this.pNormalMaterial;
        }
        this.bSeeThroughActivated = bActivate;

        // SetNeighbours
        foreach(var pNeighbor in this.NeighborList)
        {
            pNeighbor.SetNeighborOfSeeThrough(bActivate);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    void SetNeighborOfSeeThrough(bool bActivate)
    {
        if (this.GetComponent<Renderer>() == null)
            return;

        if (this.bSeeThroughActivated)
            return;

        if (bActivate)
        {
            if (this.pSeeThroughMaterial != null)
            {
                this.GetComponent<Renderer>().sharedMaterial = this.pSeeThroughMaterial;
                //this.renderer.sharedMaterial.SetFloat("_Alpha", 1.0f);
            }
        }
        else
        {
            this.GetComponent<Renderer>().sharedMaterial = this.pNormalMaterial;
        }
    }

	//-----------------------------------------------------------------------------------------------------------------
    public void SetColor(Color color)
    {
        this.xOriginalColor = color;
        this.AdjustColor(this.xOriginalColor);
    }

    //-----------------------------------------------------------------------------------------------------------------
    public void RevertColor()
    {
        this.AdjustColor(this.xOriginalColor);
    }

	//-----------------------------------------------------------------------------------------------------------------
	public void AdjustColor(Color color)
	{
        //if (this.renderer != null &&
        //    this.renderer.sharedMaterial != null &&
        //    this.renderer.sharedMaterial.color != color)
        //{
        //    this.renderer.sharedMaterial.color = color;
        //}
        if (this.pNormalMaterial != null)
            this.pNormalMaterial.color = color;

        if (this.pSeeThroughMaterial != null)
            this.pSeeThroughMaterial.color = color;
    }

    //-----------------------------------------------------------------------------------------------------------------
    void OnCollisionEnter(Collision pCollision)
    {
        foreach (ContactPoint contact in pCollision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
		
		Calc(pCollision);
    }
	
	//-----------------------------------------------------------------------------------------------------------------
   	void OnCollisionExit(Collision pCollision)
	{
		var pPlayer = pCollision.gameObject.GetComponent<Player>();
        if (pPlayer != null)
        {
           	this.SetContact(false);
		}
	}
	
	//-----------------------------------------------------------------------------------------------------------------
   	void Calc(Collision pCollision)
	{
		var pPlayer = pCollision.gameObject.GetComponent<Player>();
		if (pPlayer != null)
        {
			this.AdjustColor(Color.magenta);
			var vectorToCollition = pCollision.contacts[0].normal;
			
			var pPlayerPhysicObject = pCollision.gameObject.GetComponent<PhysicObjekt>();
			var s = Vector3.Dot(vectorToCollition.normalized, pPlayerPhysicObject.GravityDirection.normalized);	
			if (s > Definitions.BallTouchingTheGroundThresholdAsDotProductResult)
			{
                this.SetContact(true);
            }
        }
	}

    //-----------------------------------------------------------------------------------------------------------------
}
