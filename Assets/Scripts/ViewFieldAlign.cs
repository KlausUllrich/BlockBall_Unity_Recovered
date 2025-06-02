using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ViewFieldAlign : MonoBehaviour
{
    public Transform TransformToAlign;
    public Camera CameraToAlign;

    // -------------------------------------------------------------------------------------------

    // -------------------------------------------------------------------------------------------
    void Awake()
    {
        BlockBall.Debug.ASSERT(this.TransformToAlign != null);
        BlockBall.Debug.ASSERT(this.CameraToAlign != null);
    }

    // -------------------------------------------------------------------------------------------
    void LateUpdate()
    {
        if (this.TransformToAlign != null && this.CameraToAlign != null)
        {
            var vPos = this.TransformToAlign.position;
            this.transform.position = vPos;

            var vDist = vPos - this.CameraToAlign.transform.position;
            var qRot = Quaternion.LookRotation(vDist.normalized, this.CameraToAlign.transform.up);

            this.transform.localScale = new Vector3(1, 1, vDist.magnitude);
            this.transform.rotation = qRot;
        }
    }

    // -------------------------------------------------------------------------------------------

}
