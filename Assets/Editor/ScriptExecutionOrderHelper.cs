using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ScriptExecutionOrderHelper
{
    static ScriptExecutionOrderHelper()
    {
        // This will run when the editor loads and ensure script execution order is set
        EditorApplication.delayCall += SetScriptExecutionOrder;
    }

    private static void SetScriptExecutionOrder()
    {
        // Set execution order for key scripts to ensure new physics logic overrides old physics logic
        SetExecutionOrder(typeof(PhysicObjekt), -100); // Old physics script - early execution
        SetExecutionOrder(typeof(PlayerCameraController), -50); // Player input - after old physics
        SetExecutionOrder(typeof(PhysicsObjectWrapper), 100); // New physics wrapper - late execution to override
        Debug.Log("Script Execution Order updated for physics migration scripts.");
    }

    private static void SetExecutionOrder(System.Type scriptType, int order)
    {
        string scriptName = scriptType.Name;
        MonoScript script = FindMonoScript(scriptType);
        if (script != null)
        {
            int currentOrder = MonoImporter.GetExecutionOrder(script);
            if (currentOrder != order)
            {
                MonoImporter.SetExecutionOrder(script, order);
                Debug.Log($"Set execution order for {scriptName} to {order}");
            }
            else
            {
                Debug.Log($"Execution order for {scriptName} is already {order}");
            }
        }
        else
        {
            Debug.LogWarning($"Could not find script {scriptName} to set execution order.");
        }
    }

    private static MonoScript FindMonoScript(System.Type scriptType)
    {
        string[] guids = AssetDatabase.FindAssets($"t:Script {scriptType.Name}");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (script != null && script.GetClass() == scriptType)
            {
                return script;
            }
        }
        return null;
    }
}
