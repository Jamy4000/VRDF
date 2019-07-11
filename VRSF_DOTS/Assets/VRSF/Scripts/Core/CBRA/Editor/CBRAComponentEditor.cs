#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

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

        private bool _showUnityEvents = false;

        #region MAIN_VARIABLES
        // The References for the UnityEvents
        private SerializedProperty _deviceToUse;
        private SerializedProperty _interactionType;
        private SerializedProperty _buttonHand;
        private SerializedProperty _buttonToUse;

        private SerializedProperty _touchThumbPosition;
        private SerializedProperty _clickThumbPosition;

        private SerializedProperty _isTouchingThreshold;
        private SerializedProperty _isClickingThreshold;
        #endregion MAIN_VARIABLES

        #region UNITY_EVENTS
        // The References for the UnityEvents
        private SerializedProperty _onButtonStartTouchingProperty;
        private SerializedProperty _onButtonStopTouchingProperty;
        private SerializedProperty _onButtonIsTouchingProperty;

        private SerializedProperty _onButtonStartClickingProperty;
        private SerializedProperty _onButtonStopClickingProperty;
        private SerializedProperty _onButtonIsClickingProperty;
        #endregion UNITY_EVENTS

        #endregion


        #region PUBLIC_METHODS
        public virtual void OnEnable()
        {
            // We set the buttonActionChoser reference
            _cbraTarget = (ControllersButtonResponseAssigner)target;

            _deviceToUse = serializedObject.FindProperty("DeviceUsingCBRA");
            _interactionType = serializedObject.FindProperty("InteractionType");
            _buttonHand = serializedObject.FindProperty("ButtonHand");
            _buttonToUse = serializedObject.FindProperty("ButtonToUse");

            _touchThumbPosition = serializedObject.FindProperty("TouchThumbPosition");
            _clickThumbPosition = serializedObject.FindProperty("ClickThumbPosition");

            _isTouchingThreshold = serializedObject.FindProperty("IsTouchingThreshold");
            _isClickingThreshold = serializedObject.FindProperty("IsClickingThreshold");

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

            DisplayPersonalizedEditor();
        }

        private void DisplayPersonalizedEditor()
        {
            // We give a list of device that can use this feature to the user.
            if (!DisplayDeviceToUse()) return;

            EditorGUILayout.Space();

            // We check that the user has set a good value for the Interaction Type. if not, we don't display the rest of the parameters.
            if (!DisplayInteractionTypeParameters()) return;

            EditorGUILayout.Space();

            // We check that the user has set a good value for the Interaction Type. if not, we don't display the rest of the parameters.
            if (!DisplayHandParameters()) return;

            EditorGUILayout.Space();

            // We check that the user has set a good value for the Interaction Type. if not, we don't display the rest of the parameters.
            if (!DisplayButtonToUseParameter()) return;

            EditorGUILayout.Space();

            DisplayInteractionTypesMessages();

            EditorGUILayout.Space();

            CheckThumbPos();

            EditorGUILayout.Space();

            DisplayInteractionEvents();
        }

        private void DisplayInteractionTypesMessages()
        {
            switch (_cbraTarget.InteractionType)
            {
                case EControllerInteractionType.TOUCH:
                    if (!HandleTouchDisplay())
                        return;

                    break;

                case EControllerInteractionType.CLICK:
                    if (!HandleClickDisplay())
                        return;

                    break;

                case EControllerInteractionType.ALL:
                    bool touchParam = HandleTouchDisplay();
                    EditorGUILayout.Space();
                    bool clickParam = HandleClickDisplay();
                    if (!touchParam || !clickParam)
                        return;

                    break;

                default:
                    _buttonToUse.enumValueIndex = (int)EControllerInteractionType.NONE;
                    EditorGUILayout.HelpBox("Please chose a valid Interaction Type.", MessageType.Error);
                    return;
            }
        }

        private bool DisplayButtonToUseParameter()
        {
            EditorGUILayout.LabelField("The button you wanna use for this feature", EditorStyles.miniBoldLabel);

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing button to use");
            EditorGUILayout.PropertyField(_buttonToUse);
            CheckEndChanges();

            if (_buttonToUse.intValue == (int)EControllersButton.NONE)
            {
                EditorGUILayout.HelpBox("The Button to use cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }

        private void DisplayThumbPosition(EControllerInteractionType interactionType)
        {
            switch (interactionType)
            {
                case EControllerInteractionType.CLICK:
                    EditorGUILayout.LabelField("Thumb Position to use for click", EditorStyles.miniBoldLabel);

                    Rect ourRect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginProperty(ourRect, GUIContent.none, _clickThumbPosition);
                    EditorGUI.BeginChangeCheck();

                    Undo.RecordObject(_cbraTarget, "Changing click thumb pos");
                    _clickThumbPosition.intValue = (int)(EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Click Position", _cbraTarget.ClickThumbPosition);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

                    EditorGUI.EndProperty();
                    EditorGUILayout.EndHorizontal();
                    CheckEndChanges();

                    ourRect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginProperty(ourRect, GUIContent.none, _isClickingThreshold);
                    EditorGUI.BeginChangeCheck();

                    Undo.RecordObject(_cbraTarget, "Changing click threshold");
                    _isClickingThreshold.floatValue = EditorGUILayout.Slider("Click Detection Threshold", _cbraTarget.IsClickingThreshold, 0.0f, 1.0f);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

                    EditorGUI.EndProperty();
                    EditorGUILayout.EndHorizontal();
                    CheckEndChanges();

                    break;

                case EControllerInteractionType.TOUCH:
                    EditorGUILayout.LabelField("Thumb Position to use for touch", EditorStyles.miniBoldLabel);

                    ourRect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginProperty(ourRect, GUIContent.none, _touchThumbPosition);
                    EditorGUI.BeginChangeCheck();

                    Undo.RecordObject(_cbraTarget, "Changing touch thumb pos");
                    _touchThumbPosition.intValue = (int)(EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Touch Position", _cbraTarget.TouchThumbPosition);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

                    EditorGUI.EndProperty();
                    EditorGUILayout.EndHorizontal();
                    CheckEndChanges();

                    ourRect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginProperty(ourRect, GUIContent.none, _isTouchingThreshold);
                    EditorGUI.BeginChangeCheck();

                    Undo.RecordObject(_cbraTarget, "Changing touch thumb pos");
                    _isTouchingThreshold.floatValue = EditorGUILayout.Slider("Touch Detection Threshold", _cbraTarget.IsTouchingThreshold, 0.0f, 1.0f);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

                    EditorGUI.EndProperty();
                    EditorGUILayout.EndHorizontal();
                    CheckEndChanges();

                    break;
            }
        }

        // Last thing to handle : MultipleObjectEditing using if (proipertyChange) then assign value

        private bool DisplayDeviceToUse()
        {
            Undo.RecordObject(_cbraTarget, "Changing DeviceToUse");

            EditorGUILayout.LabelField("Device using this CBRA", EditorStyles.miniBoldLabel);

            Rect ourRect = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginProperty(ourRect, GUIContent.none, _deviceToUse);
            EditorGUI.BeginChangeCheck();

            _deviceToUse.intValue = (int)(EDevice)EditorGUILayout.EnumFlagsField("Device List", _cbraTarget.DeviceUsingCBRA);

            EditorGUI.EndProperty();
            EditorGUILayout.EndHorizontal();
            CheckEndChanges();

            if (_deviceToUse.intValue == (int)EDevice.NONE)
            {
                EditorGUILayout.HelpBox("The Device type cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }

        private bool DisplayInteractionTypeParameters()
        {
            EditorGUILayout.LabelField("Type of Interaction with the Controller", EditorStyles.miniBoldLabel);

            Rect ourRect = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginProperty(ourRect, GUIContent.none, _interactionType);
            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(_cbraTarget, "Changing InteractionType");
            _interactionType.intValue = (int)(EControllerInteractionType)EditorGUILayout.EnumFlagsField("Interaction Type", _cbraTarget.InteractionType);

            EditorGUI.EndProperty();
            EditorGUILayout.EndHorizontal();
            CheckEndChanges();


            if (_interactionType.intValue == (int)EControllerInteractionType.NONE)
            {
                EditorGUILayout.HelpBox("The Interaction type cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }


        private bool DisplayHandParameters()
        {
            EditorGUILayout.LabelField("Hand using this feature", EditorStyles.miniBoldLabel);

            Undo.RecordObject(_cbraTarget, "Changing Hand");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_buttonHand);
            CheckEndChanges();

            if (_buttonHand.intValue != (int)EHand.LEFT && _buttonHand.intValue != (int)EHand.RIGHT)
            {
                EditorGUILayout.HelpBox("The Hand cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }


        private void CheckThumbPos()
        {
            if (_buttonToUse.enumValueIndex != (int)EControllersButton.TOUCHPAD)
                return;

            if ((_cbraTarget.InteractionType == EControllerInteractionType.CLICK && _cbraTarget.ClickThumbPosition == EThumbPosition.NONE) ||
                (_cbraTarget.InteractionType == EControllerInteractionType.TOUCH && _cbraTarget.TouchThumbPosition == EThumbPosition.NONE) ||
                (_cbraTarget.InteractionType == EControllerInteractionType.ALL &&
                    (_cbraTarget.TouchThumbPosition == EThumbPosition.NONE || _cbraTarget.ClickThumbPosition == EThumbPosition.NONE)))
            {
                EditorGUILayout.HelpBox("Please chose a valid Thumb Position.", MessageType.Error);
            }
        }


        private void DisplayInteractionEvents()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _showUnityEvents = EditorGUILayout.ToggleLeft("Display UnityEvents for Click/Touch Events", _showUnityEvents);

            if (_showUnityEvents)
            {
                if ((_cbraTarget.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
                {
                    EditorGUILayout.Space();
                    DisplayTouchEvents();
                }
                if ((_cbraTarget.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
                {
                    EditorGUILayout.Space();
                    DisplayClickEvents();
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
            Undo.RecordObject(_cbraTarget, "Changing stop touch events");
            EditorGUILayout.PropertyField(_onButtonStopTouchingProperty);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing is touching events");
            EditorGUILayout.PropertyField(_onButtonIsTouchingProperty);
            CheckEndChanges();
        }


        private void DisplayClickEvents()
        {
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing start clicking events");
            EditorGUILayout.PropertyField(_onButtonStartClickingProperty);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing stop clicking events");
            EditorGUILayout.PropertyField(_onButtonStopClickingProperty);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_cbraTarget, "Changing is clicking events");
            EditorGUILayout.PropertyField(_onButtonIsClickingProperty);
            CheckEndChanges();
        }


        private bool HandleTouchDisplay()
        {
            switch (_cbraTarget.ButtonToUse)
            {
                case EControllersButton.A_BUTTON:
                case EControllersButton.B_BUTTON:
                case EControllersButton.X_BUTTON:
                case EControllersButton.Y_BUTTON:
                case EControllersButton.THUMBREST:
                    DisplayOculusWarning();
                    return true;

                case EControllersButton.TOUCHPAD:
                    DisplayThumbPosition(EControllerInteractionType.TOUCH);
                    return true;

                case EControllersButton.NONE:
                    EditorGUILayout.HelpBox("Touch : Please chose a valid Button to Use.", MessageType.Error);
                    return false;

                case EControllersButton.TRIGGER:
                    return true;

                default:
                    DisplayTouchError();
                    return false;
            }
        }

        private bool HandleClickDisplay()
        {
            switch (_cbraTarget.ButtonToUse)
            {
                case EControllersButton.A_BUTTON:
                case EControllersButton.B_BUTTON:
                case EControllersButton.X_BUTTON:
                case EControllersButton.Y_BUTTON:
                    DisplayOculusWarning();
                    return true;

                case EControllersButton.BACK_BUTTON:
                    DisplaySingleControllerWarning();
                    return true;

                case EControllersButton.MENU:
                    if (_cbraTarget.ButtonHand == EHand.RIGHT)
                        DisplayViveWarning();
                    return true;

                case EControllersButton.THUMBREST:
                    DisplayClickError();
                    return false;

                case EControllersButton.TOUCHPAD:
                    DisplayThumbPosition(EControllerInteractionType.CLICK);
                    return true;

                case EControllersButton.NONE:
                    EditorGUILayout.HelpBox("Click : Please chose a valid Button to se.", MessageType.Error);
                    return false;

                default:
                    return true;
            }
        }

        private void DisplayOculusWarning()
        {
            EditorGUILayout.HelpBox("This feature will only be available for the Oculus Touch Controllers, " +
                "as the A, B, X, Y button and the Thumbrests doesn't exist on the other type Controllers.", MessageType.Warning);
        }

        private void DisplaySingleControllerWarning()
        {
            EditorGUILayout.HelpBox("This feature will only be available for the Oculus Go and the Gear VR Controllers, " +
                "as the Back button doesn't exist on the other type Controllers.", MessageType.Warning);
        }

        private void DisplayViveWarning()
        {
            EditorGUILayout.HelpBox("This feature will only be available for the Vive Controllers, " +
                "as the Right Menu button cannot be use with the other type Controllers.", MessageType.Warning);
        }

        private void DisplayTouchError()
        {
            EditorGUILayout.HelpBox("This Button cannot be use for the Touch Interaction, as it is not available " +
                "on the current devices.", MessageType.Error);
        }

        private void DisplayClickError()
        {
            EditorGUILayout.HelpBox("This Button cannot be use for the Click Interaction, as it is not available " +
                "on the current devices.", MessageType.Error);
        }

        void CheckEndChanges()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Utils/Add CBRA Object", priority = 0)]
        [MenuItem("VRSF/Utils/Add CBRA Object", priority = 0)]
        public static void AddCBRAObject(MenuCommand menuCommand)
        {
            var cbraObject = new GameObject("CBRA");
            cbraObject.transform.SetParent(Selection.activeTransform);
            cbraObject.AddComponent<ControllersButtonResponseAssigner>();
            Selection.SetActiveObjectWithContext(cbraObject, menuCommand.context);
        }
        #endregion
    }
}
#endif