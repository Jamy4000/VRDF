using UnityEditor;
using UnityEngine;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.VRRotation
{
    [CustomEditor(typeof(LinearRotationAuthoring))]
    public class LinearRotationEditorScript : Editor
    {
        private LinearRotationAuthoring _lra;

        public void OnEnable()
        {
            _lra = (LinearRotationAuthoring)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_lra.UseDecelerationEffect)
            {
                var decelerationSpeedProp = serializedObject.FindProperty("DecelerationFactor");

                EditorGUI.BeginChangeCheck();

                var newValue = EditorGUILayout.Slider("Deceleration Factor", decelerationSpeedProp.floatValue, 0.1f, 10.0f);

                if (EditorGUI.EndChangeCheck())
                {
                    decelerationSpeedProp.floatValue = newValue;
                    serializedObject.ApplyModifiedProperties();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_lra);
                }
            }

            _lra.GetComponent<VRInteractionAuthoring>().ButtonToUse = Core.Inputs.EControllersButton.TOUCHPAD;
        }

        /// <summary>
        /// Add a linear rotation gameObject in the scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Move Around/Rotation/Add Linear Rotation", priority = 3)]
        [MenuItem("VRSF/Move Around/Rotation/Add Linear Rotation", priority = 3)]
        public static void AddLinearRotationObject(MenuCommand menuCommand)
        {
            var linearRot = new GameObject("Linear Rotation");
            Undo.RegisterCreatedObjectUndo(linearRot, "Adding linearRotation");
            linearRot.transform.SetParent(Selection.activeTransform);
            linearRot.AddComponent<LinearRotationAuthoring>();
            linearRot.AddComponent<VRInteractionAuthoring>();
            Selection.SetActiveObjectWithContext(linearRot, menuCommand.context);
        }
    }
}