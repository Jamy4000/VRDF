using UnityEngine;
using UnityEditor;

namespace VRSF.Core.FadingEffect
{
    /// <summary>
    /// Display the Fade In On SetupVR Ready bool in the CameraFadeAuthoring script, and depending on the value,
    /// display the slider for the time before fade in starts
    /// </summary>
    [CustomEditor(typeof(CameraFadeAuthoring))]
    public class CameraFadeAuthoringInspector : Editor
    {
        private CameraFadeAuthoring _target;
        private SerializedProperty _fadeInOnSetupVRReady;
        private SerializedProperty _timeOffsetBeforeFadeIn;

        public virtual void OnEnable()
        {
            _target = (CameraFadeAuthoring)target;
            _fadeInOnSetupVRReady = serializedObject.FindProperty("FadeInOnSetupVRReady");
            _timeOffsetBeforeFadeIn = serializedObject.FindProperty("TimeBeforeFirstFadeIn");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("OnSetupVRReady Parameters", EditorStyles.boldLabel);

            Rect ourRect = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginProperty(ourRect, GUIContent.none, _fadeInOnSetupVRReady);
            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(_target, "Changing _fadeInOnSetupVRReady");
            _fadeInOnSetupVRReady.boolValue = EditorGUILayout.ToggleLeft("Fade In On SetupVR Ready", _fadeInOnSetupVRReady.boolValue);

            EditorGUI.EndProperty();
            EditorGUILayout.EndHorizontal();
            CheckEndChanges();

            if (_fadeInOnSetupVRReady.boolValue)
            {
                ourRect = EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginProperty(ourRect, GUIContent.none, _timeOffsetBeforeFadeIn);
                EditorGUI.BeginChangeCheck();

                Undo.RecordObject(_target, "Changing _timeOffsetBeforeFadeIn");
                _timeOffsetBeforeFadeIn.floatValue = EditorGUILayout.Slider("Time before Fade In Start", _timeOffsetBeforeFadeIn.floatValue, 0.0f, 20.0f);

                EditorGUI.EndProperty();
                EditorGUILayout.EndHorizontal();
                CheckEndChanges();

                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This GameObject will be destroyed on Start.", MessageType.Warning);
        }

        private void CheckEndChanges()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(_target);
            }
        }
    }
}