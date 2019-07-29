using UnityEditor;
using UnityEngine;

namespace VRSF.MoveAround.VRRotation
{
    [CustomEditor(typeof(NonLinearRotationAuthoring))]
    public class NonLinearRotationEditorScript : Editor
    {
        /// <summary>
        /// Add a linear rotation gameObject in the scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Move Around/Rotation/Add Non Linear Rotation", priority = 3)]
        [MenuItem("VRSF/Move Around/Rotation/Add Non Linear Rotation", priority = 3)]
        public static void AddNonLinearRot(MenuCommand menuCommand)
        {
            var nonLinearRot = new GameObject("Non Linear Rotation");
            Undo.RegisterCreatedObjectUndo(nonLinearRot, "Adding linearRotation");
            nonLinearRot.transform.SetParent(Selection.activeTransform);
            nonLinearRot.AddComponent<NonLinearRotationAuthoring>();
            Selection.SetActiveObjectWithContext(nonLinearRot, menuCommand.context);
        }
    }
}