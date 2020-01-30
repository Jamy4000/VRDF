#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// Handle the Options in the Inspector for the ControllersButtonResponseAssigner 
    /// </summary>
    [CustomEditor(typeof(VRInteractionAuthoring), true)]
    [CanEditMultipleObjects]
    public class VRInteractionSetEditor : Editor
    {
        #region PRIVATE_VARIABLES
        // The reference to the target
        private VRInteractionAuthoring _interactionSet;

        // The References for the UnityEvents
        private SerializedProperty _deviceToUse;
        private SerializedProperty _interactionType;
        private SerializedProperty _usePositionForTouch;
        private SerializedProperty _buttonHand;
        private SerializedProperty _buttonToUse;

        private SerializedProperty _touchThumbPosition;
        private SerializedProperty _clickThumbPosition;

        private SerializedProperty _isTouchingThreshold;
        private SerializedProperty _isClickingThreshold;

        #endregion


        #region PUBLIC_METHODS
        public virtual void OnEnable()
        {
            // We set the buttonActionChoser reference
            _interactionSet = (VRInteractionAuthoring)target;

            _deviceToUse = serializedObject.FindProperty("DeviceUsingFeature");
            _interactionType = serializedObject.FindProperty("InteractionType");
            _usePositionForTouch = serializedObject.FindProperty("UseThumbPositionForTouch");
            _buttonHand = serializedObject.FindProperty("ButtonHand");
            _buttonToUse = serializedObject.FindProperty("ButtonToUse");

            _touchThumbPosition = serializedObject.FindProperty("TouchThumbPosition");
            _clickThumbPosition = serializedObject.FindProperty("ClickThumbPosition");

            _isTouchingThreshold = serializedObject.FindProperty("IsTouchingThreshold");
            _isClickingThreshold = serializedObject.FindProperty("IsClickingThreshold");
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            serializedObject.Update();

            EditorGUILayout.Space();

            DisplayPersonalizedEditor();

            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayPersonalizedEditor()
        {
            // We give a list of device that can use this feature to the user.
            if (!DisplayDeviceToUse()) return;

            EditorGUILayout.Space();

            // We check that the user has set a good value for the Interaction Type. if not, we don't display the rest of the parameters.
            if (!DisplayInteractionTypeParameters()) return;

            EditorGUILayout.Space();

            // We add the boolean for touch using thumb position if relevant
            if (Utils.InteractionSetupHelper.FlagHasOculusDevice(_interactionSet.DeviceUsingFeature) && _interactionSet.InteractionType.HasFlag(EControllerInteractionType.TOUCH))
            {
                DisplayUseThumbPosBool();
                EditorGUILayout.Space();
            }

            // We check that the user has set a good value for the Interaction Type. if not, we don't display the rest of the parameters.
            if (!DisplayHandParameters()) return;

            EditorGUILayout.Space();

            // We check that the user has set a good value for the Interaction Type. if not, we don't display the rest of the parameters.
            if (!DisplayButtonToUseParameter()) return;

            EditorGUILayout.Space();

            DisplayInteractionTypesMessages();

            EditorGUILayout.Space();

            CheckThumbPos();
        }

        private void DisplayUseThumbPosBool()
        {
            EditorGUILayout.LabelField("OCULUS ONLY : Should we use the thumb position for the touch feature", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_interactionSet, "Use Thumb position for Touch");
            EditorGUILayout.PropertyField(_usePositionForTouch);
            CheckEndChanges();
        }

        private bool DisplayButtonToUseParameter()
        {
            EditorGUILayout.LabelField("Button calling the events", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_interactionSet, "Changing button to use");
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

                    Undo.RecordObject(_interactionSet, "Changing click thumb pos");
                    _clickThumbPosition.intValue = (int)(EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Click Position", _interactionSet.ClickThumbPosition);

                    EditorGUI.EndProperty();
                    EditorGUILayout.EndHorizontal();
                    CheckEndChanges();

                    ourRect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginProperty(ourRect, GUIContent.none, _isClickingThreshold);
                    EditorGUI.BeginChangeCheck();

                    Undo.RecordObject(_interactionSet, "Changing click threshold");
                    _isClickingThreshold.floatValue = EditorGUILayout.Slider("Click Detection Threshold", _interactionSet.IsClickingThreshold, 0.0f, 1.0f);

                    EditorGUI.EndProperty();
                    EditorGUILayout.EndHorizontal();
                    CheckEndChanges();

                    break;

                case EControllerInteractionType.TOUCH:
                    EditorGUILayout.LabelField("Thumb Position to use for touch", EditorStyles.miniBoldLabel);

                    ourRect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginProperty(ourRect, GUIContent.none, _touchThumbPosition);
                    EditorGUI.BeginChangeCheck();

                    Undo.RecordObject(_interactionSet, "Changing touch thumb pos");
                    _touchThumbPosition.intValue = (int)(EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Touch Position", _interactionSet.TouchThumbPosition);

                    EditorGUI.EndProperty();
                    EditorGUILayout.EndHorizontal();
                    CheckEndChanges();

                    ourRect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginProperty(ourRect, GUIContent.none, _isTouchingThreshold);
                    EditorGUI.BeginChangeCheck();

                    Undo.RecordObject(_interactionSet, "Changing touch thumb pos");
                    _isTouchingThreshold.floatValue = EditorGUILayout.Slider("Touch Detection Threshold", _interactionSet.IsTouchingThreshold, 0.0f, 1.0f);

                    EditorGUI.EndProperty();
                    EditorGUILayout.EndHorizontal();
                    CheckEndChanges();

                    break;
            }
        }

        private bool DisplayDeviceToUse()
        {
            EditorGUILayout.LabelField("Device using this feature", EditorStyles.boldLabel);

            Rect ourRect = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginProperty(ourRect, GUIContent.none, _deviceToUse);
            EditorGUI.BeginChangeCheck();

            var oldVal = _deviceToUse.intValue;
            var newVal = (int)(EDevice)EditorGUILayout.EnumFlagsField("Device List", _interactionSet.DeviceUsingFeature);

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndProperty();
            if (CheckEndChanges(oldVal, newVal))
            {
                Undo.RecordObject(_interactionSet, "Changing DeviceToUse");
                _deviceToUse.intValue = newVal;
            }

            if (newVal == (int)EDevice.NONE)
            {
                EditorGUILayout.HelpBox("The Device type cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }

        private bool DisplayInteractionTypeParameters()
        {
            EditorGUILayout.LabelField("Type of Interaction with the Controller", EditorStyles.boldLabel);

            Rect ourRect = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginProperty(ourRect, GUIContent.none, _interactionType);
            EditorGUI.BeginChangeCheck();

            var oldVal = _interactionType.intValue;
            var newVal = (int)(EControllerInteractionType)EditorGUILayout.EnumFlagsField("Interaction Type", _interactionSet.InteractionType);
            EditorGUI.EndProperty();
            EditorGUILayout.EndHorizontal();

            if (CheckEndChanges(oldVal, newVal))
            {
                Undo.RecordObject(_interactionSet, "Changing DeviceToUse");
                _interactionType.intValue = newVal;
            }

            if (_interactionType.intValue == (int)EControllerInteractionType.NONE)
            {
                EditorGUILayout.HelpBox("The Interaction type cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }


        private bool DisplayHandParameters()
        {
            EditorGUILayout.LabelField("Hand using this Button", EditorStyles.boldLabel);

            Undo.RecordObject(_interactionSet, "Changing Hand");
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

            if ((_interactionSet.InteractionType == EControllerInteractionType.CLICK && _interactionSet.ClickThumbPosition == EThumbPosition.NONE) ||
                (_interactionSet.InteractionType == EControllerInteractionType.TOUCH && _interactionSet.TouchThumbPosition == EThumbPosition.NONE) ||
                (_interactionSet.InteractionType == EControllerInteractionType.ALL && (_interactionSet.TouchThumbPosition == EThumbPosition.NONE || _interactionSet.ClickThumbPosition == EThumbPosition.NONE)))
            {
                EditorGUILayout.HelpBox("Please chose a valid Thumb Position.", MessageType.Error);
            }
        }

        private void DisplayInteractionTypesMessages()
        {
            switch (_interactionSet.InteractionType)
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

        private bool HandleTouchDisplay()
        {
            switch (_interactionSet.ButtonToUse)
            {
                case EControllersButton.A_BUTTON:
                case EControllersButton.B_BUTTON:
                    DisplayOculusWarning();
                    _interactionSet.ButtonHand = EHand.RIGHT;
                    serializedObject.ApplyModifiedProperties();
                    return true;
                case EControllersButton.X_BUTTON:
                case EControllersButton.Y_BUTTON:
                    DisplayOculusWarning();
                    _interactionSet.ButtonHand = EHand.LEFT;
                    serializedObject.ApplyModifiedProperties();
                    return true;

                case EControllersButton.THUMBREST:
                case EControllersButton.GRIP:
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
            switch (_interactionSet.ButtonToUse)
            {
                case EControllersButton.A_BUTTON:
                case EControllersButton.B_BUTTON:
                    DisplayOculusWarning();
                    _interactionSet.ButtonHand = EHand.RIGHT;
                    serializedObject.ApplyModifiedProperties();
                    return true;
                case EControllersButton.X_BUTTON:
                case EControllersButton.Y_BUTTON:
                    DisplayOculusWarning();
                    _interactionSet.ButtonHand = EHand.LEFT;
                    serializedObject.ApplyModifiedProperties();
                    return true;

                case EControllersButton.BACK_BUTTON:
                    DisplaySingleControllerWarning();
                    return true;

                case EControllersButton.MENU:
                    if (_interactionSet.ButtonHand == EHand.RIGHT)
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
            EditorGUILayout.HelpBox("This feature will only be available for the Oculus Touch Controllers.", MessageType.Warning);
        }

        private void DisplaySingleControllerWarning()
        {
            EditorGUILayout.HelpBox("This feature will only be available for the Oculus Go and the Gear VR Controllers, " +
                "as the Back button doesn't exist on the other type Controllers.", MessageType.Warning);
        }

        private void DisplayViveWarning()
        {
            EditorGUILayout.HelpBox("This menu button is not available for the right hand on the Oculus Devices.", MessageType.Warning);
        }

        private void DisplayTouchError()
        {
            EditorGUILayout.HelpBox("This Button cannot be use for the Touch Interaction.", MessageType.Error);
        }

        private void DisplayClickError()
        {
            EditorGUILayout.HelpBox("This Button cannot be use for the Click Interaction", MessageType.Error);
        }

        private bool CheckEndChanges(int oldValue = -1, int newValue = -1)
        {
            if (EditorGUI.EndChangeCheck() || oldValue != newValue)
            {
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(_interactionSet);
                return true;
            }
            return false;
        }
        #endregion
    }
}
#endif