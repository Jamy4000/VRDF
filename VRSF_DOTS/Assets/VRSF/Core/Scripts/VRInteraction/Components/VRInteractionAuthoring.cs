using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

namespace VRSF.Core.VRInteractions
{
    public class VRInteractionAuthoring : MonoBehaviour
    {
        /// <summary>
        /// The Devices that are using this VR Interaction
        /// </summary>
        [HideInInspector] [SerializeField] public EDevice DeviceUsingFeature = EDevice.ALL;

        /// <summary>
        /// The type of Interaction you want to use
        /// </summary>
        [HideInInspector] [SerializeField] public EControllerInteractionType InteractionType = EControllerInteractionType.NONE;

        /// <summary>
        /// The hand on which the button to use is situated
        /// </summary>
        [HideInInspector] [SerializeField] public EHand ButtonHand;

        /// <summary>
        /// Used by the Oculus Devices, as the touch feature is only raised when touching the top part of the thumbstick.
        /// If this is true, the touch event will be raised even if the finger isn't on the touchable part of the thumbstick.
        /// </summary>
        [HideInInspector] [SerializeField] public bool UseThumbPositionForTouch = true;

        /// <summary>
        /// The button you wanna use for the Action
        /// </summary>
        [HideInInspector] [SerializeField] public EControllersButton ButtonToUse = EControllersButton.NONE;


        /// <summary>
        /// The position of the thumb you wanna use for the Touch Action
        /// </summary>
        [HideInInspector] [SerializeField] public EThumbPosition TouchThumbPosition = EThumbPosition.NONE;

        /// <summary>
        /// The position of the thumb you wanna use for the Click Action
        /// </summary>
        [HideInInspector] [SerializeField] public EThumbPosition ClickThumbPosition = EThumbPosition.NONE;


        /// <summary>
        /// At which threshold is the IsTouching event raised ? Absolute Value between 0 and 1
        /// </summary>
        [HideInInspector] [SerializeField] public float IsTouchingThreshold = 0.0f;

        /// <summary>
        /// At which threshold is the IsClicking event raised ? Absolute Value between 0 and 1
        /// </summary>
        [HideInInspector] [SerializeField] public float IsClickingThreshold = 0.0f;
    }
}
