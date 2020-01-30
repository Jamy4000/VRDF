using UnityEditor;
using UnityEngine;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.VRRotation
{
    [CustomEditor(typeof(NonLinearRotationAuthoring))]
    public class NonLinearRotationEditorScript : Editor
    {
        private NonLinearRotationAuthoring _lra;

        public void OnEnable()
        {
            _lra = (NonLinearRotationAuthoring)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _lra.GetComponent<VRInteractionAuthoring>().ButtonToUse = Core.Inputs.EControllersButton.TOUCHPAD;
        }

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
            nonLinearRot.AddComponent<VRInteractionAuthoring>();
            Selection.SetActiveObjectWithContext(nonLinearRot, menuCommand.context);
        }
    }
}