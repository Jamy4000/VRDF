using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
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
        [HideInInspector] public Vector2Variable ThumbPos = null;

        // BoolVariable to check
        [HideInInspector] public BoolVariable IsTouching = null;
        [HideInInspector] public BoolVariable IsClicking = null;
    }
}