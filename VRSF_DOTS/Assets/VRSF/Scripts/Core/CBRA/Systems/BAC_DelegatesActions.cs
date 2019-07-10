using System;
using System.Collections;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    public class BAC_DelegatesActions
    {
        public readonly BACGeneralComponent BACGeneral;
        public readonly BACCalculationsComponent BACCalculations;

        public BAC_DelegatesActions(BACGeneralComponent bacGeneral, BACCalculationsComponent bacCalcul)
        {
            BACGeneral = bacGeneral;
            BACCalculations = bacCalcul;
        }

        IEnumerator WaitForTimer(Action toInvoke)
        {
            yield return new WaitForSeconds(BACGeneral.BACTimer.TimerThreshold + 0.01f);

            if (BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer))
                toInvoke.Invoke();
        }

        /// <summary>
        /// Method called when user click the specified button
        /// </summary>
        public void StartActionDown(ButtonClickEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (ParametersAreOK(eventButton.HandInteracting, eventButton.ButtonInteracting))
            {
                // Check if we use a timer and if the timer is ready
                if (BACGeneral.BACTimer != null && !BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer))
                {
                    Action newAction = new Action(() => ActionDown());
                    BACCalculations.StartCoroutine(WaitForTimer(newAction));
                }
                else
                {
                    ActionDown();
                }
            }


            void ActionDown()
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (BACCalculations.ThumbPos != null && BACGeneral.ClickThreshold > 0.0f)
                {
                    BACCalculations.UnclickEventWasRaised = false;

                    switch (BACGeneral.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(BACGeneral.RightClickThumbPosition, BACGeneral.OnButtonStartClicking, BACGeneral.ClickThreshold, BACCalculations.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(BACGeneral.LeftClickThumbPosition, BACGeneral.OnButtonStartClicking, BACGeneral.ClickThreshold, BACCalculations.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    BACGeneral.OnButtonStartClicking.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user release the specified button
        /// </summary>
        public void StartActionUp(ButtonUnclickEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (ParametersAreOK(eventButton.HandInteracting, eventButton.ButtonInteracting) && (BACGeneral.BACTimer == null || BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer)))
            {
                BACGeneral.OnButtonStopClicking.Invoke();
                BACCalculations.StopAllCoroutines();
            }
        }


        /// <summary>
        /// Method called when user start touching the specified button
        /// </summary>
        public void StartActionTouched(ButtonTouchEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (ParametersAreOK(eventButton.HandInteracting, eventButton.ButtonInteracting))
            {
                // Check if we use a timer and if the timer is ready
                if (BACGeneral.BACTimer != null && !BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer))
                {
                    Action newAction = new Action(() => ActionTouched());
                    BACGeneral.StartCoroutine(WaitForTimer(newAction));
                }
                else
                {
                    ActionTouched();
                }
            }

            /// <summary>
            /// Actual Method checking if everwting is ok
            /// </summary>
            void ActionTouched()
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (BACCalculations.ThumbPos != null && BACGeneral.TouchThreshold > 0.0f)
                {
                    BACCalculations.UntouchedEventWasRaised = false;

                    switch (BACGeneral.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(BACGeneral.RightTouchThumbPosition, BACGeneral.OnButtonStartTouching, BACGeneral.TouchThreshold, BACCalculations.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(BACGeneral.LeftTouchThumbPosition, BACGeneral.OnButtonStartTouching, BACGeneral.TouchThreshold, BACCalculations.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    BACGeneral.OnButtonStartTouching.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user stop touching the specified button
        /// </summary>
        public void StartActionUntouched(ButtonUntouchEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            // Check as well if we use a timer and, if so, if the timer is ready
            if (ParametersAreOK(eventButton.HandInteracting, eventButton.ButtonInteracting) && (BACGeneral.BACTimer == null || BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer)))
            {
                BACGeneral.OnButtonStopTouching.Invoke();
                BACGeneral.StopAllCoroutines();
            }
        }


        private bool ParametersAreOK(EHand handInteracting, EControllersButton buttonInteracting)
        {
            return BACGeneral.ButtonHand == handInteracting && BACGeneral.ActionButton == buttonInteracting && BACCalculations.CanBeUsed;
        }
    }
}