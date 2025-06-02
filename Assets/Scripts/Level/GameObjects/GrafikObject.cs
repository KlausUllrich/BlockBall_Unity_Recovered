using UnityEngine;
using System.Collections;

public class GrafikObjekt : BaseObject {
		
    //-----------------------------------------------------------------------------------------------------------------
    public MeshFilter MeshFilter { get; private set; }

    //-----------------------------------------------------------------------------------------------------------------
    protected virtual void Awake()
    {
        this.MeshFilter = this.gameObject.GetComponent<MeshFilter>();
    }
	
	//-----------------------------------------------------------------------------------------------------------------
}
