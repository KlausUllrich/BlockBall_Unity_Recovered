using UnityEngine;
using System.Xml;
using System.Collections;

public class GravitySwitchHelper : MonoBehaviour
{
    //-----------------------------------------------------------------------------------------------------------------
    public GravitySwitch GravitySwitch;
    public Vector3 LineStart;
    public Vector3 LineEnd;
    public bool AttractionGravity;

    //-----------------------------------------------------------------------------------------------------------------
    private Vector3 vWorldLineStart;
    private Vector3 vWorldLineEnd;

    //-----------------------------------------------------------------------------------------------------------------
    void Start()
    {
        this.TransformGravityAxisToWorldSpace();
    }

    //-----------------------------------------------------------------------------------------------------------------
    void TransformGravityAxisToWorldSpace()
    {
        var pTransform = this.transform;

        vWorldLineStart = this.LineStart;
        vWorldLineEnd.Scale(pTransform.localScale);

        vWorldLineEnd = this.LineEnd;
        vWorldLineEnd.Scale(pTransform.localScale);

        vWorldLineStart = pTransform.rotation * vWorldLineStart;
        vWorldLineEnd = pTransform.rotation * vWorldLineEnd;

        vWorldLineStart += pTransform.position;
        vWorldLineEnd += pTransform.position;
    }

    //-----------------------------------------------------------------------------------------------------------------
    void OnTriggerStay(Collider pCollider)
    {
        var pPlayer = pCollider.gameObject.GetComponent<Player>();
        if (pPlayer != null)
        {
            var vPlayerPos = pPlayer.transform.position;
            if (true/*pPlayer.HasGroundContact()*/)
            {
                if (this.GetComponent<Collider>().bounds.Contains(vPlayerPos))
                {
                    var vNewDir = this.CalculateNewGravityDirection(vPlayerPos);
                    pPlayer.SetGravityDirection(vNewDir);
                }
                else
                {
                    // when on to gravity switches this is not good as it brings false results ! -> should be affected by one of the two but not clip !
                    //pPlayer.ClipGravity();
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    void OnTriggerExit(Collider pCollider)
    {
        var pPlayer = pCollider.gameObject.GetComponent<Player>();
        if (pPlayer != null)
        {
            var vPlayerPos = pPlayer.transform.position;
            if (/*check todo ground contact*/ true)
            {
                pPlayer.ClipGravity();
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    Vector3	CalculateNewGravityDirection(Vector3 vObjectCenter)
    {
	    // Find the perpendicular "cut"-vector to GravityAxis from ObjectCenter
	    Vector3 vClosestPoint = this.ClosestPointOnLine(this.vWorldLineStart, this.vWorldLineEnd, vObjectCenter);

	    // Find new Gravity direction
	    Vector3 vNewDirection = vClosestPoint - vObjectCenter;

        if (this.AttractionGravity)
            return vNewDirection.normalized;
	    else
            return -vNewDirection.normalized;
    }

    //-----------------------------------------------------------------------------------------------------------------
    Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
    {
        var vVector1 = vPoint - vA;
        var vVector2 = (vB - vA).normalized;
     
        var d = Vector3.Distance(vA, vB);
        var t = Vector3.Dot(vVector2, vVector1);
     
        if (t <= 0)
            return vA;
     
        if (t >= d)
            return vB;
     
        var vVector3 = vVector2 * t;
     
        var vClosestPoint = vA + vVector3;
     
        return vClosestPoint;
    }

    //-----------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        TransformGravityAxisToWorldSpace();

        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.vWorldLineStart, this.vWorldLineEnd);

        var vBallPoint = this.GetComponent<Collider>().bounds.center;
        var vStart = ClosestPointOnLine(this.vWorldLineStart, this.vWorldLineEnd, vBallPoint);
	    Vector3 vDirection = vStart - vBallPoint;
        if (this.AttractionGravity)
            vDirection = vDirection.normalized;
	    else
            vDirection = -vDirection.normalized;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(vStart, vDirection);
    }
#endif
    //-----------------------------------------------------------------------------------------------------------------
}
