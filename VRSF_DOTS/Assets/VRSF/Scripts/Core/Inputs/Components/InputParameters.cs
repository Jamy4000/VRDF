using ScriptableFramework.Variables;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Used to handle the Right and Left controllers Inputs
    /// </summary>
    public class InputParameters
    {
        public BoolVariable TriggerClick;
        public BoolVariable TriggerTouch;
        public FloatVariable TriggerSqueezeValue;

        public BoolVariable TouchpadClick;
        public BoolVariable TouchpadTouch;
        public Vector2Variable ThumbPosition;

        public BoolVariable GripClick;
        public BoolVariable GripTouch;
        public FloatVariable GripSqueezeValue;

        public VRInputsBoolean ClickBools;
        public VRInputsBoolean TouchBools;

        public InputParameters(VRInputsBoolean clickBools, VRInputsBoolean touchBools, Vector2Variable thumbPosition, FloatVariable triggerSqueezeValue, FloatVariable gripSqueezeValue)
        {
            ClickBools = clickBools;
            TouchBools = touchBools;
            ThumbPosition = thumbPosition;

            TriggerClick = clickBools.Get("TriggerIsDown");
            TriggerTouch = touchBools.Get("TriggerIsTouching");
            TriggerSqueezeValue = triggerSqueezeValue;

            TouchpadClick = clickBools.Get("ThumbIsDown");
            TouchpadTouch = touchBools.Get("ThumbIsTouching");
            GripSqueezeValue = gripSqueezeValue;

            GripClick = clickBools.Get("GripIsDown");
            GripTouch = touchBools.Get("GripIsTouching");
        }
    }
}