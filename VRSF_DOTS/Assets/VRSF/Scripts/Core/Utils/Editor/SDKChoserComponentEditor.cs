#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VRSF.Core.Utils.ButtonActionChoser;

namespace VRSF.Utils.Editor
{
    /// <summary>
    /// Handle the Options in the Inspector for the class that extend ButtonActionChoser 
    /// </summary>
    [CustomEditor(typeof(SDKChoserComponent), true)]
    public class SDKChoserComponentEditor : UnityEditor.Editor
    {
        private SDKChoserComponent _component;

        private void OnEnable()
        {
            // We set the buttonActionChoser reference
            _component = (SDKChoserComponent)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update(); 

            base.DrawDefaultInspector();

            EditorGUILayout.Space();

            DisplaySDKsToggles();

        }

        private void DisplaySDKsToggles()
        {
            GUILayoutOption[] options = { GUILayout.MaxWidth(100.0f), GUILayout.MinWidth(10.0f) };

            EditorGUILayout.LabelField("Chose which SDK is using this script", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            _component.UseVive = EditorGUILayout.ToggleLeft("OpenVR", _component.UseVive, options);
            _component.UseRift = EditorGUILayout.ToggleLeft("OVR", _component.UseRift, options);
            _component.UseWMR = EditorGUILayout.ToggleLeft("WMR", _component.UseWMR, options);
            _component.UseSimulator = EditorGUILayout.ToggleLeft("Simulator", _component.UseSimulator, options);

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif