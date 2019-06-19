using System.Collections.Generic;
using UnityEngine.Events;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    /// <summary>
    /// Check if the thumbstick is above a c
    /// </summary>
    public class ThumstickChecker
    {
        public readonly UnityEvent EventToRaise;
        public readonly Dictionary<EHand, EThumbPosition> ThumbPositions;
        public readonly float InteractionThreshold;
        public readonly bool EventWasRaised;
        public readonly EHand ButtonHand;
        public readonly UnityEngine.Vector2 ThumbPosValue;

        public ThumstickChecker(BACGeneralComponent bacGeneral, BACCalculationsComponent bacCalc, EControllerInteractionType interactionType, UnityEvent eventToRaise = null)
        {
            switch (interactionType)
            {
                case EControllerInteractionType.CLICK:
                    EventToRaise = eventToRaise ?? bacGeneral.OnButtonIsClicking;

                    ThumbPositions = new Dictionary<EHand, EThumbPosition>
                    {
                        { EHand.LEFT, bacGeneral.LeftClickThumbPosition },
                        { EHand.RIGHT, bacGeneral.RightClickThumbPosition }
                    };

                    InteractionThreshold = bacGeneral.ClickThreshold;
                    EventWasRaised = bacCalc.UnclickEventWasRaised;
                    break;

                case EControllerInteractionType.TOUCH:
                    EventToRaise = eventToRaise ?? bacGeneral.OnButtonIsTouching;

                    ThumbPositions = new Dictionary<EHand, EThumbPosition>
                    {
                        { EHand.LEFT, bacGeneral.LeftTouchThumbPosition },
                        { EHand.RIGHT, bacGeneral.RightTouchThumbPosition }
                    };

                    InteractionThreshold = bacGeneral.TouchThreshold;
                    EventWasRaised = bacCalc.UntouchedEventWasRaised;
                    break;
            }

            ButtonHand = bacGeneral.ButtonHand;
            ThumbPosValue = bacCalc.ThumbPos.Value;
        }
    }
}