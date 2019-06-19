using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    /// <summary>
    /// Handle the update for when you want to delay or stop a Button Action Update System feature before or after a timer.
    /// 
    /// If this timer is updated before the threshold, your BAC feature will launch the BAC events Callbacks
    /// if the user press and release the button before the end of the timer threshold.
    /// 
    /// If this timer is updated after the threshold, your BAC feature will launch the BAC events Callbacks
    /// only after the time specified if the user press the button until the end of the timer threshold.
    /// </summary>
    public class BACTimerUpdateSystem : ComponentSystem
    {
        struct Filter
        {
            public BACTimerComponent BACTimer;
            public BACGeneralComponent BAC_Comp;
            public BACCalculationsComponent BAC_Calc;
        }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            OnSetupVRReady.Listeners += Init;
        }

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
                        e.BACTimer._OldTouchingState = true;
                    }
                    else if (e.BACTimer._OldTouchingState)
                    {
                        e.BACTimer._OldTouchingState = false;
                        e.BACTimer.StartCoroutine(OnStopInteractingCallback(e.BACTimer));
                    }

                    // If we use the click event and the user is clicking on the button
                    if (e.BAC_Calc.IsClicking != null && e.BAC_Calc.IsClicking.Value)
                    {
                        StartActionIsClicking(e);
                        e.BACTimer._OldClickingState = true;
                    }
                    else if (e.BACTimer._OldClickingState)
                    {
                        e.BACTimer._OldClickingState = false;
                        e.BACTimer.StartCoroutine(OnStopInteractingCallback(e.BACTimer));
                    }
                }
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            foreach (var e in GetEntities<Filter>())
            {
                // Remove the listeners for the ThumbCheckEvent if it's not null
                e.BACTimer.ThumbCheckEvent?.RemoveListener(e.BACTimer.ThumbEventAction);
                e.BACTimer.ThumbCheckEvent = null;
            }
            OnSetupVRReady.Listeners -= Init;
        }

        /// <summary>
        /// Update timer based on a fixed unscaled delta time when user interact with the button
        /// </summary>
        /// <param name="e"></param>
        private void IsInteractingCallback(BACTimerComponent timer)
        {
            timer._Timer += Time.deltaTime;
        }

        /// <summary>
        /// Waiting for two frame on stop interacting so all the systems using on stop interacting 
        /// can finish what they're doing with the final value of the timer.
        /// </summary>
        /// <param name="e">The entity in which the timer is</param>
        /// <returns></returns>
        private IEnumerator OnStopInteractingCallback(BACTimerComponent timer)
        {
            yield return new WaitForEndOfFrame();
            // We reset the timers stuffs
            timer._Timer = 0.0f;
        }


        /// <summary>
        /// Override of StartActionIsClicking as the timer doesn't need to check the presence of a timer or if the timer is ready.
        /// </summary>
        /// <param name="e"></param>
        private void StartActionIsClicking(Filter e)
        {
            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (e.BAC_Calc.ThumbPos != null)
            {
                e.BAC_Calc.UnclickEventWasRaised = BACUpdateSystem.CheckTouchpad
                (
                    new ThumstickChecker(e.BAC_Comp, e.BAC_Calc, EControllerInteractionType.CLICK, e.BAC_Comp.BACTimer.ThumbCheckEvent),
                    ref e.BAC_Calc.ClickActionBeyondThreshold
                );
            }
            else
            {
                IsInteractingCallback(e.BACTimer);
            }
        }

        /// <summary>
        /// Override of StartActionIsTouching as the timer doesn't need to check the presence of a timer or if the timer is ready.
        /// </summary>
        /// <param name="e"></param>
        private void StartActionIsTouching(Filter e)
        {
            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (e.BAC_Calc.ThumbPos != null)
            {
                e.BAC_Calc.UntouchedEventWasRaised = BACUpdateSystem.CheckTouchpad
                (
                    new ThumstickChecker(e.BAC_Comp, e.BAC_Calc, EControllerInteractionType.TOUCH, e.BAC_Comp.BACTimer.ThumbCheckEvent),
                    ref e.BAC_Calc.TouchActionBeyondThreshold
                );
            }
            else
            {
                IsInteractingCallback(e.BAC_Comp.BACTimer);
            }
        }


        /// <summary>
        /// Check if the timer is under or above the timing required by the threshold, as specified with the IsUpdatedBeforeThreshold bool
        /// </summary>
        /// <param name="BACTimer">Reference to a BAC Timer component to access its data</param>
        /// <returns>true if the user clicking is under or above the time limit, as specified with the IsUpdatedBeforeThreshold bool</returns>
        public static bool TimerIsReady(BACTimerComponent BACTimer)
        {
            // If the system is updated before the threshold and the timer is inferior to the time limit OR
            // If the system is is updated before the threshold and the timer is superior to the time limit
            return (BACTimer.IsUpdatedBeforeThreshold && BACTimer._Timer < BACTimer.TimerThreshold) ||
                   (!BACTimer.IsUpdatedBeforeThreshold && BACTimer._Timer > BACTimer.TimerThreshold);
        }


        private void Init(OnSetupVRReady info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                e.BAC_Comp.BACTimer = e.BACTimer;
                // if we use a thumbstick
                if (e.BAC_Comp.ActionButton == EControllersButton.TOUCHPAD && e.BACTimer.ThumbCheckEvent == null)
                {
                    // We create a new event that will be use in the CheckThumbstick method
                    e.BACTimer.ThumbCheckEvent = new UnityEvent();
                    e.BACTimer.ThumbEventAction = delegate { IsInteractingCallback(e.BACTimer); };
                    e.BACTimer.ThumbCheckEvent.AddListenerExtend(e.BACTimer.ThumbEventAction);
                }
            }
        }
    }
}