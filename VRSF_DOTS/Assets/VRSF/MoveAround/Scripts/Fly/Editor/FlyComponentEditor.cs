using UnityEditor;
using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(FlyModeAuthoring))]
    public class FlyComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("To use the Fly System, you only need to set the parameters of the Fly Mode Authoring and the VRInteractions Authoring.\n" +
                "The response to the buttons you chose are already handled in script.", MessageType.Info);
        }

        /// <summary>
        /// Add a linear rotation gameObject in the scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Move Around/Add Fly Mode", priority = 3)]
        [MenuItem("VRSF/Move Around/Add Fly Mode", priority = 3)]
        public static void AddFlyMode(MenuCommand menuCommand)
        {
            var flyModeObject = new GameObject("Fly Mode");
            Undo.RegisterCreatedObjectUndo(flyModeObject, "Adding fly mode");
            flyModeObject.transform.SetParent(Selection.activeTransform);
            flyModeObject.AddComponent<FlyModeAuthoring>();
            Selection.SetActiveObjectWithContext(flyModeObject, menuCommand.context);
        }
    }
}