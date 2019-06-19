using ScriptableFramework.Utils;
using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Contain all the references to Variables, RuntimeSets and GameEvents for the Inputs in VR
    /// </summary>
    public class InputVariableContainer : ScriptableSingleton<InputVariableContainer>
    {
        [Multiline(10)]
        public string DeveloperDescription = "";

        [Header("Runtime Dictionnary to get the BoolVariable (Touch and Click) for each button")]
        public VRInputsBoolean RightClickBoolean;
        public VRInputsBoolean LeftClickBoolean;
        public VRInputsBoolean RightTouchBoolean;
        public VRInputsBoolean LeftTouchBoolean;
        
        [Header("Vector2Variable for the Thumb position")]
        public Vector2Variable RightThumbPosition;
        public Vector2Variable LeftThumbPosition;

        [Header("FloatVariable for the Trigger Squeeze Values")]
        public FloatVariable RightTriggerSqueezeValue;
        public FloatVariable LeftTriggerSqueezeValue;

        [Header("FloatVariable for the Grip Squeeze Values")]
        public FloatVariable RightGripSqueezeValue;
        public FloatVariable LeftGripSqueezeValue;
    }
}