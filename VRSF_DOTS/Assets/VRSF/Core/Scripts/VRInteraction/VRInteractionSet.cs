using System;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

namespace VRSF.Core.VRInteraction
{
    [Serializable]
    public struct VRInteractionSet
    {
        /// <summary>
        /// The Devices that are using this VR Interaction
        /// </summary>
        public EDevice DeviceUsingCBRA;

        /// <summary>
        /// The type of Interaction you want to use
        /// </summary>
        public EControllerInteractionType InteractionType;

        /// <summary>
        /// The hand on which the button to use is situated
        /// </summary>
        public EHand ButtonHand;

        /// <summary>
        /// The button you wanna use for the Action
        /// </summary>
        public EControllersButton ButtonToUse;


        /// <summary>
        /// The position of the thumb you wanna use for the Touch Action
        /// </summary>
        public EThumbPosition TouchThumbPosition;

        /// <summary>
        /// The position of the thumb you wanna use for the Click Action
        /// </summary>
        public EThumbPosition ClickThumbPosition;


        /// <summary>
        /// At which threshold is the IsTouching event raised ? Absolute Value between 0 and 1
        /// </summary>
        public float IsTouchingThreshold;

        /// <summary>
        /// At which threshold is the IsClicking event raised ? Absolute Value between 0 and 1
        /// </summary>
        public float IsClickingThreshold;
    }
}
