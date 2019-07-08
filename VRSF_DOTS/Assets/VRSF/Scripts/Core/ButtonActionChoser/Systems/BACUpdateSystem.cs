using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    public class BACUpdateSystem : ComponentSystem
    {
        public struct Filter
        {
            public BACGeneralComponent BAC_Comp;
            public BACCalculationsComponent BAC_Calc;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BAC_Calc.ActionButtonIsReady && e.BAC_Calc.CanBeUsed)
                {
                    // If we use the touch event and the user is touching on the button
                    if (e.BAC_Calc.IsTouching != null && e.BAC_Calc.IsTouching.Value)
                    {
                        StartActionIsTouching(e);
                    }
                    // If we use the click event and the user is clicking on the button
                    if (e.BAC_Calc.IsClicking != null && e.BAC_Calc.IsClicking.Value)
                    {
                        StartActionIsClicking(e);
                    }
                }
            }
        }
        #endregion


        #region PRIVATE_VARIABLES
        /// <summary>
        /// Method called when user stop touching the specified button
        /// It's virtual as the timer update system doen't need to check if the timer is ready.
        /// </summary>
        public virtual void StartActionIsClicking(Filter entity)
        {
            if (entity.BAC_Comp.BACTimer != null && !BACTimerUpdateSystem.TimerIsReady(entity.BAC_Comp.BACTimer))
                return;
            
            // if we use the Thumb, we need to check its position on the Touchpad/Touchpad
            if (entity.BAC_Calc.ThumbPos != null)
            {
                entity.BAC_Calc.UnclickEventWasRaised = CheckTouchpad
                (
                    new ThumstickChecker(entity.BAC_Comp, entity.BAC_Calc, EControllerInteractionType.CLICK), 
                    ref entity.BAC_Calc.ClickActionBeyondThreshold
                );
            }
            else
            {
                entity.BAC_Comp.OnButtonIsClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when the user stop touching the specified button.
        /// It's virtual as the timer update system doen't need to check if the timer is ready.
        /// </summary>
        public virtual void StartActionIsTouching(Filter entity)
        {
            if (entity.BAC_Comp.BACTimer != null && !BACTimerUpdateSystem.TimerIsReady(entity.BAC_Comp.BACTimer))
                return;
            
            // if we use the Thumb, we need to check its position on the Touchpad/Touchpad
            if (entity.BAC_Calc.ThumbPos != null)
            {
                entity.BAC_Calc.UntouchedEventWasRaised = CheckTouchpad
                (
                    new ThumstickChecker(entity.BAC_Comp, entity.BAC_Calc, EControllerInteractionType.TOUCH), 
                    ref entity.BAC_Calc.TouchActionBeyondThreshold
                );
            }
            else
            {
                entity.BAC_Comp.OnButtonIsTouching.Invoke();
            }
        }

        /// <summary>
        /// Check if the position of the thumbstick is still within the threshold specified by the user, and If not, we raise the event.
        /// </summary>
        /// <param name="entity"></param>
        public static bool CheckTouchpad(ThumstickChecker thumbstickChecker, ref bool actionAboveThreshold)
        {
            bool oldState = actionAboveThreshold;

            actionAboveThreshold = HandleThumbPosition.CheckThumbPosition(thumbstickChecker.ThumbPositions[thumbstickChecker.ButtonHand], thumbstickChecker.EventToRaise, thumbstickChecker.InteractionThreshold, thumbstickChecker.ThumbPosValue);
            
            if (oldState && !actionAboveThreshold && !thumbstickChecker.EventWasRaised)
            {
                thumbstickChecker.EventToRaise.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}