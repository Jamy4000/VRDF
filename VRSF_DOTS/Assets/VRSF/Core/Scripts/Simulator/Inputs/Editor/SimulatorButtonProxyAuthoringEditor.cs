using UnityEngine;
using UnityEditor;

namespace VRSF.Core.Simulator
{
    /// <summary>
    /// Display a warning message on the SimulatorButtonProxyAuthoring script
    /// </summary>
    [CustomEditor(typeof(SimulatorButtonProxyAuthoring))]
    public class SimulatorButtonProxyAuthoringEditor : Editor
    {
        // The reference to the target
        private SimulatorButtonProxyAuthoring _proxyTarget;
         
        private SerializedProperty _mouseButtonToUse;
        private SerializedProperty _keyboardButtonToUse;

        public virtual void OnEnable()
        {
            // We set the buttonActionChoser reference
            _proxyTarget = (SimulatorButtonProxyAuthoring)target;

            _mouseButtonToUse = serializedObject.FindProperty("SimulationMouseButton");
            _keyboardButtonToUse = serializedObject.FindProperty("SimulationKeyCode");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Button to use for Simulation", EditorStyles.miniBoldLabel);

            if (_proxyTarget.UseMouseButton)
            {
                Rect ourRect = EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginProperty(ourRect, GUIContent.none, _mouseButtonToUse);
                EditorGUI.BeginChangeCheck();

                Undo.RecordObject(_proxyTarget, "Changing click button");
                _mouseButtonToUse.intValue = (int)(EMouseButton)EditorGUILayout.EnumPopup("Mouse Click to use", _proxyTarget.SimulationMouseButton);

                EditorGUI.EndProperty();
                EditorGUILayout.EndHorizontal();
                CheckEndChanges();

                if (_proxyTarget.SimulationMouseButton == EMouseButton.NONE)
                    EditorGUILayout.HelpBox("This VR Interaction won't be used by the Simulator. Consider removing this component if not used.", MessageType.Warning);
                if (_proxyTarget.SimulationMouseButton == EMouseButton.RIGHT_CLICK)
                    EditorGUILayout.HelpBox("The Right Click is used by the Simulator Rotation System, are you sure you want to use it as well here ?", MessageType.Warning);
            }
            else
            {
                Rect ourRect = EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginProperty(ourRect, GUIContent.none, _mouseButtonToUse);
                EditorGUI.BeginChangeCheck();

                Undo.RecordObject(_proxyTarget, "Changing keyboard button");
                _keyboardButtonToUse.intValue = (int)(KeyCode)EditorGUILayout.EnumPopup("Keyboard Button to use", _proxyTarget.SimulationKeyCode);

                EditorGUI.EndProperty();
                EditorGUILayout.EndHorizontal();
                CheckEndChanges();

                if (_proxyTarget.SimulationKeyCode == KeyCode.None)
                    EditorGUILayout.HelpBox("This VR Interaction won't be used by the Simulator. Consider removing this component if not used.", MessageType.Warning);
            }
        }

        private void CheckEndChanges()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(_proxyTarget);
            }
        }
    }
}