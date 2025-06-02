using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GravitySwitchHelper))]
public class GravitySwitchHelperInspector : Editor
{
    // -------------------------------------------------------------------------------------------
    public override void OnInspectorGUI()
    {
        var pScript = (GravitySwitchHelper)this.target;

        DrawDefaultInspector();
    }

    // -------------------------------------------------------------------------------------------
}
