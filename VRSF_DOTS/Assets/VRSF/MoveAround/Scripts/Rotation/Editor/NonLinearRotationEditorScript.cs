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
        public static void AddCBRAObject(MenuCommand menuCommand)
        {
            var cbraObject = new GameObject("Non Linear Rotation");
            cbraObject.transform.SetParent(Selection.activeTransform);
            cbraObject.AddComponent<NonLinearRotationAuthoring>();
            Selection.SetActiveObjectWithContext(cbraObject, menuCommand.context);
        }
    }
}