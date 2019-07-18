using System.Collections;
using System.Collections.Generic;
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

                var newValue = EditorGUILayout.Slider("Deceleration Factor", decelerationSpeedProp.floatValue, 0.1f, 5.0f);

                if (EditorGUI.EndChangeCheck())
                {
                    decelerationSpeedProp.floatValue = newValue;
                    serializedObject.ApplyModifiedProperties();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_lra);
                }
            }
        }
    }
}