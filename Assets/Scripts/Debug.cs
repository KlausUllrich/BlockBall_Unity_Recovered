using System.Diagnostics;

namespace BlockBall
{
    /// <summary>
    /// Debug utility class for assertions and logging
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Verifies that a condition is true. If the condition is false, logs an error message and throws an exception.
        /// </summary>
        /// <param name="bCondition">The condition to verify</param>
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

        /// <summary>
        /// Verifies that a condition is true. If the condition is false, logs the specified error message and throws an exception.
        /// </summary>
        /// <param name="bCondition">The condition to verify</param>
        /// <param name="sMessage">The message to log if the assertion fails</param>
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
    }
}