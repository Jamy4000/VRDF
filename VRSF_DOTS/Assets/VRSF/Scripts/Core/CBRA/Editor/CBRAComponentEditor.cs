#if UNITY_EDITOR
using System;
using UnityEditor;
using VRSF.Core.Controllers;
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

            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayPersonalizedEditor()
        {
            // We check that the user has set a good value for the Interaction Type. if not, we don't display the rest of the parameters.
            if (!DisplayInteractionTypeParameters()) return;

            EditorGUILayout.Space();

            // We check that the user has set a good value for the Interaction Type. if not, we don't display the rest of the parameters.
            if (!DisplayHandParameters()) return;

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("The button you wanna use for this feature", EditorStyles.miniBoldLabel);
            Undo.RecordObject(_cbraTarget, "Changing button to use");
            _cbraTarget.ButtonToUse = (EControllersButton)EditorGUILayout.EnumPopup("Button to use", _cbraTarget.ButtonToUse);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

            EditorGUILayout.Space();

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
                    _cbraTarget.ButtonToUse = EControllersButton.NONE;
                    EditorGUILayout.HelpBox("Please chose a valid Interaction Type.", MessageType.Error);
                    return;
            }

            CheckThumbPos();

            DisplayInteractionEvents();
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


        private void DisplayThumbPosition(EControllerInteractionType interactionType)
        {
            switch (interactionType)
            {
                case EControllerInteractionType.CLICK:
                    EditorGUILayout.LabelField("Thumb Position to use for click", EditorStyles.miniBoldLabel);

                    Undo.RecordObject(_cbraTarget, "Changing click thumb pos");
                    _cbraTarget.ClickThumbPosition = (EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Click Position", _cbraTarget.ClickThumbPosition);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

                    Undo.RecordObject(_cbraTarget, "Changing click threshold");
                    _cbraTarget.IsClickingThreshold = EditorGUILayout.Slider("Click Detection Threshold", _cbraTarget.IsClickingThreshold, 0.0f, 1.0f);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);
                    break;

                case EControllerInteractionType.TOUCH:
                    EditorGUILayout.LabelField("Thumb Position to use for touch", EditorStyles.miniBoldLabel);

                    Undo.RecordObject(_cbraTarget, "Changing touch thumb pos");
                    _cbraTarget.TouchThumbPosition = (EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Touch Position", _cbraTarget.TouchThumbPosition);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

                    Undo.RecordObject(_cbraTarget, "Changing touch thumb pos");
                    _cbraTarget.IsTouchingThreshold = EditorGUILayout.Slider("Touch Detection Threshold", _cbraTarget.IsTouchingThreshold, 0.0f, 1.0f);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);
                    break;
            }
        }


        private bool DisplayInteractionTypeParameters()
        {
            Undo.RecordObject(_cbraTarget, "Changing InteractionType");

            EditorGUILayout.LabelField("Type of Interaction with the Controller", EditorStyles.miniBoldLabel);
            _cbraTarget.InteractionType = (EControllerInteractionType)EditorGUILayout.EnumFlagsField("Interaction Type", _cbraTarget.InteractionType);

            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

            if (_cbraTarget.InteractionType == EControllerInteractionType.NONE)
            {
                EditorGUILayout.HelpBox("The Interaction type cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }


        private bool DisplayHandParameters()
        {
            Undo.RecordObject(_cbraTarget, "Changing Hand");

            EditorGUILayout.LabelField("Hand using this feature", EditorStyles.miniBoldLabel);
            _cbraTarget.ButtonHand = (EHand)EditorGUILayout.EnumPopup("Hand", _cbraTarget.ButtonHand);

            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

            if (_cbraTarget.ButtonHand != EHand.LEFT && _cbraTarget.ButtonHand != EHand.RIGHT)
            {
                EditorGUILayout.HelpBox("The Hand cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }


        private void CheckThumbPos()
        {
            if (_cbraTarget.ButtonToUse != EControllersButton.TOUCHPAD)
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
            Undo.RecordObject(_cbraTarget, "Changing start touch events");
            EditorGUILayout.PropertyField(_onButtonStartTouchingProperty);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

            Undo.RecordObject(_cbraTarget, "Changing stop touch events");
            EditorGUILayout.PropertyField(_onButtonStopTouchingProperty);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

            Undo.RecordObject(_cbraTarget, "Changing is touching events");
            EditorGUILayout.PropertyField(_onButtonIsTouchingProperty);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);
        }


        private void DisplayClickEvents()
        {
            Undo.RecordObject(_cbraTarget, "Changing start clicking events");
            EditorGUILayout.PropertyField(_onButtonStartClickingProperty);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

            Undo.RecordObject(_cbraTarget, "Changing stop clicking events");
            EditorGUILayout.PropertyField(_onButtonStopClickingProperty);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);

            Undo.RecordObject(_cbraTarget, "Changing is clicking events");
            EditorGUILayout.PropertyField(_onButtonIsClickingProperty);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_cbraTarget);
        }
        #endregion
    }
}
#endif