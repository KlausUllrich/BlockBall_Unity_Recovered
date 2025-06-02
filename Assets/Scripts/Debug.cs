using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace BlockBall
{
public class Debug
{
	//-------------------------------------------------------------------------------------------------------------
    [Conditional("DEBUG"), Conditional("UNITY_EDITOR")]
    public static void ASSERT(bool bCondition)
    {
        if (!bCondition)
        {
            var pStackTrace = new System.Diagnostics.StackTrace(true);
			var message = "ASSERTion failed! Stacktrace: " + pStackTrace.ToString();
			UnityEngine.Debug.Log(message);
            throw new System.Exception(message);
        }
    }
	
	//-------------------------------------------------------------------------------------------------------------
    [Conditional("DEBUG"), Conditional("UNITY_EDITOR")]
    public static void ASSERT(bool bCondition, string sMessage)
    {
        if (!bCondition)
        {
            var pStackTrace = new System.Diagnostics.StackTrace(true);
			var message = "ASSERTion failed! Message: " + sMessage + " Stacktrace: " + pStackTrace.ToString();
           	UnityEngine.Debug.Log(message);
            throw new System.Exception(message);
        }
    }
	
	//-------------------------------------------------------------------------------------------------------------
}
}