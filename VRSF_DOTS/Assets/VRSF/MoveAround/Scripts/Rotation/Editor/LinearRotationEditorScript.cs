using UnityEditor;
using UnityEngine;

namespace VRSF.MoveAround.Rotation
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
        }

        /// <summary>
        /// Add a linear rotation gameObject in the scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Move Around/Rotation/Add Linear Rotation", priority = 3)]
        [MenuItem("VRSF/Move Around/Rotation/Add Linear Rotation", priority = 3)]
        public static void AddCBRAObject(MenuCommand menuCommand)
        {
            var cbraObject = new GameObject("Linear Rotation");
            cbraObject.transform.SetParent(Selection.activeTransform);
            cbraObject.AddComponent<LinearRotationAuthoring>();
            Selection.SetActiveObjectWithContext(cbraObject, menuCommand.context);
        }
    }
}