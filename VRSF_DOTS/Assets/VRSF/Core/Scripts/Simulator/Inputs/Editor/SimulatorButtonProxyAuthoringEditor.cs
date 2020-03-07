using UnityEngine;
using UnityEditor;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Display a warning message on the SimulatorButtonProxyAuthoring script
    /// </summary>
    [CustomEditor(typeof(SimulatorButtonProxyAuthoring))]
    public class SimulatorButtonProxyAuthoringEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("This GameObject will be destroyed on Start.", MessageType.Warning);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Utils/Add Simulator Button Proxy", priority = 2)]
        [MenuItem("VRSF/Utils/Add Simulator Button Proxy", priority = 2)]
        public static void AddSimulatorButtonProxyObject(MenuCommand menuCommand)
        {
            var cbraObject = new GameObject("Simulator Button Proxy");
            Undo.RegisterCreatedObjectUndo(cbraObject, "Adding new SimulatorButtonProxy");
            cbraObject.transform.SetParent(Selection.activeTransform);
            cbraObject.AddComponent<VRInteractions.VRInteractionAuthoring>();
            cbraObject.AddComponent<SimulatorButtonProxyAuthoring>();
            Selection.SetActiveObjectWithContext(cbraObject, menuCommand.context);
        }
    }
}