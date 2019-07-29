#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the Options in the Inspector for the ControllersButtonResponseAssigner 
    /// </summary>
    [CustomEditor(typeof(ControllersButtonResponseAssigner), true)]
    [CanEditMultipleObjects]
    public class CBRAComponentEditor : Editor
    {
        #region PRIVATE_VARIABLES
        // The reference to the target
        private ControllersButtonResponseAssigner _cbraTarget;
        private VRInteractions.VRInteractionAuthoring _interactionSet;

        private bool _showUnityEvents = false;

        // The References for the UnityEvents
        private SerializedProperty _onButtonStartTouchingProperty;
        private SerializedProperty _onButtonStopTouchingProperty;
        private SerializedProperty _onButtonIsTouchingProperty;

        private SerializedProperty _onButtonStartClickingProperty;
        private SerializedProperty _onButtonStopClickingProperty;
        private SerializedProperty _onButtonIsClickingProperty;

        #endregion


        #region PUBLIC_METHODS
        public virtual void OnEnable()
        {
            // We set the buttonActionChoser reference
            _cbraTarget = (ControllersButtonResponseAssigner)target;
            _interactionSet = _cbraTarget.GetComponent<VRInteractions.VRInteractionAuthoring>();

            _onButtonStartTouchingProperty = serializedObject.FindProperty("OnButtonStartTouching");
            _onButtonStopTouchingProperty = serializedObject.FindProperty("OnButtonStopTouching");
            _onButtonIsTouchingProperty = serializedObject.FindProperty("OnButtonIsTouching");

            _onButtonStartClickingProperty = serializedObject.FindProperty("OnButtonStartClicking");
            _onButtonStopClickingProperty = serializedObject.FindProperty("OnButtonStopClicking");
            _onButtonIsClickingProperty = serializedObject.FindProperty("OnButtonIsClicking");
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            serializedObject.Update();

            EditorGUILayout.Space();

            DisplayInteractionEvents();

            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayInteractionEvents()
        {
            if (_interactionSet.InteractionType == EControllerInteractionType.NONE)
            {
                EditorGUILayout.HelpBox("Provide a correct interaction type in the VRInteractionAuthoring component to display the Events.", MessageType.Info);
            }
            else
            {
                _showUnityEvents = EditorGUILayout.ToggleLeft("Display UnityEvents for Click/Touch Events", _showUnityEvents);

                if (_showUnityEvents)
                {
                    EditorGUILayout.HelpBox("The target script for thess events can't be on the same GameObject as this CBRA. If you do so, THE EVENTS WON'T BE CALLED.", MessageType.Warning);

                    if ((_interactionSet.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
                    {
                        EditorGUILayout.Space();
                        DisplayTouchEvents();
                    }
                    if ((_interactionSet.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
                    {
                        EditorGUILayout.Space();
                        DisplayClickEvents();
                    }
                }
            }
        }


        private void DisplayTouchEvents()
        {
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing start touch events");
            EditorGUILayout.PropertyField(_onButtonStartTouchingProperty);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing is touching events");
            EditorGUILayout.PropertyField(_onButtonIsTouchingProperty);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing stop touch events");
            EditorGUILayout.PropertyField(_onButtonStopTouchingProperty);
            CheckEndChanges();
        }


        private void DisplayClickEvents()
        {
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing start clicking events");
            EditorGUILayout.PropertyField(_onButtonStartClickingProperty);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing is clicking events");
            EditorGUILayout.PropertyField(_onButtonIsClickingProperty);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing stop clicking events");
            EditorGUILayout.PropertyField(_onButtonStopClickingProperty);
            CheckEndChanges();
        }

        private bool CheckEndChanges()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Utils/Add CBRA Object", priority = 2)]
        [MenuItem("VRSF/Utils/Add CBRA Object", priority = 2)]
        public static void AddCBRAObject(MenuCommand menuCommand)
        {
            var cbraObject = new GameObject("CBRA");
            Undo.RegisterCreatedObjectUndo(cbraObject, "Adding  new CBRA");
            cbraObject.transform.SetParent(Selection.activeTransform);
            cbraObject.AddComponent<ControllersButtonResponseAssigner>();
            Selection.SetActiveObjectWithContext(cbraObject, menuCommand.context);
        }
        #endregion
    }
}
#endif