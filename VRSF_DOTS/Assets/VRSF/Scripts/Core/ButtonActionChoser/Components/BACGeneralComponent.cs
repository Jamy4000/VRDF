using UnityEngine;
using UnityEngine.Events;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(BACCalculationsComponent))]
    public class BACGeneralComponent : MonoBehaviour
    {
        [Header("The type of Interaction you want to use")]
        [HideInInspector] public EControllerInteractionType InteractionType = EControllerInteractionType.NONE;
        
        [Header("The hand on which the button to use is situated")]
        [HideInInspector] public EHand ButtonHand = EHand.NONE;

        [Header("The button you wanna use for the Action")]
        [HideInInspector] public EControllersButton ActionButton = EControllersButton.NONE;

        [Header("The position of the Thumb to start this feature")]
        [HideInInspector] public EThumbPosition LeftTouchThumbPosition = EThumbPosition.NONE;
        [HideInInspector] public EThumbPosition RightTouchThumbPosition = EThumbPosition.NONE;
        [HideInInspector] public EThumbPosition LeftClickThumbPosition = EThumbPosition.NONE;
        [HideInInspector] public EThumbPosition RightClickThumbPosition = EThumbPosition.NONE;

        [Header("The Thresholds for the Thumb on the Controller")]
        [HideInInspector] public float TouchThreshold = 0.0f;
        [HideInInspector] public float ClickThreshold = 0.0f;
        
        [Header("The UnityEvents called when the user is Touching")]
        [HideInInspector] public UnityEvent OnButtonStartTouching = new UnityEvent();
        [HideInInspector] public UnityEvent OnButtonStopTouching = new UnityEvent();
        [HideInInspector] public UnityEvent OnButtonIsTouching = new UnityEvent();

        [Header("The UnityEvents called when the user is Clicking")]
        [HideInInspector] public UnityEvent OnButtonStartClicking = new UnityEvent();
        [HideInInspector] public UnityEvent OnButtonStopClicking = new UnityEvent();
        [HideInInspector] public UnityEvent OnButtonIsClicking = new UnityEvent();

        // Set within the BACTimerUpdateSystem on Start.
        [HideInInspector] public BACTimerComponent BACTimer;
    }
}