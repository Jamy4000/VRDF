using UnityEditor;
using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(FlyBoundariesAuthoring))]
    public class FlyBoundariesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Boundaries are shown in Scene view if the Material is not null AND if MinAvatarPos or MaxAvatarPos are different from a Vector3.zero.", MessageType.Info);
            EditorGUILayout.HelpBox("WARNING : You need to have Gizmos activated in your Scene View !", MessageType.Warning);
        }

        /// <summary>
        /// Add a linear rotation gameObject in the scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Move Around/Fly Mode/Without Boundaries", priority = 3)]
        [MenuItem("VRSF/Move Around/Fly Mode/Without Boundaries", priority = 3)]
        public static void AddFlyModeWithoutBoundaries(MenuCommand menuCommand)
        {
            var flyModeObject = new GameObject("Fly Mode");
            Undo.RegisterCreatedObjectUndo(flyModeObject, "Adding fly mode");
            flyModeObject.transform.SetParent(Selection.activeTransform);
            flyModeObject.AddComponent<FlyModeAuthoring>();
            Selection.SetActiveObjectWithContext(flyModeObject, menuCommand.context);
        }

        /// <summary>
        /// Add a linear rotation gameObject in the scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Move Around/Fly Mode/With Boundaries", priority = 3)]
        [MenuItem("VRSF/Move Around/Fly Mode/With Boundaries", priority = 3)]
        public static void AddFlyModeWithBoundaries(MenuCommand menuCommand)
        {
            var flyModeObject = new GameObject("Fly Mode");
            Undo.RegisterCreatedObjectUndo(flyModeObject, "Adding fly mode");
            flyModeObject.transform.SetParent(Selection.activeTransform);
            flyModeObject.AddComponent<FlyModeAuthoring>();
            flyModeObject.AddComponent<FlyBoundariesAuthoring>();
            Selection.SetActiveObjectWithContext(flyModeObject, menuCommand.context);
        }
    }
}