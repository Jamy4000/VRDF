using Unity.Mathematics;
using UnityEngine;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    public class BACCalculationsComponent : MonoBehaviour
    {
        [HideInInspector] public bool ActionButtonIsReady = false;
        [HideInInspector] public bool ParametersAreInvalid = false;
        [HideInInspector] public bool CorrectSDK = true;
        [HideInInspector] public bool IsSetup = false;
        public bool CanBeUsed = true;

        // To keep track of the event that were raised, used for the features that use the Touchpad
        [HideInInspector] public bool ClickActionBeyondThreshold;
        [HideInInspector] public bool TouchActionBeyondThreshold;
        [HideInInspector] public bool UntouchedEventWasRaised;
        [HideInInspector] public bool UnclickEventWasRaised;

        // For SDKs Specific ActionButton 
        [HideInInspector] public bool IsUsingOculusButton;
        [HideInInspector] public bool IsUsingViveButton;

        // Thumb Parameters
        [HideInInspector] public float2 ThumbPos;

        // BoolVariable to check
        [HideInInspector] public bool IsTouching;
        [HideInInspector] public bool IsClicking;
    }
}