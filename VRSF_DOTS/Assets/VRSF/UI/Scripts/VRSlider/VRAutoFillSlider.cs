using VRSF.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRSF.Core.VRInteractions;
using VRSF.Core.Events;
using VRSF.Core.Raycast;

namespace VRSF.UI
{
    /// <summary>
    /// Handle the references and setup for the GameEvents, GameEventListeners and boxCollider of the VRAutoFillSlider
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRAutoFillSlider : Slider
    {
        #region PUBLIC_VARIABLES
        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;

        [Tooltip("If this slider can be click using a Raycast and the trigger of your controller.")]
        [SerializeField] public bool LaserClickable = true;

        [Tooltip("If this slider can be click using the meshcollider of your controller.")]
        [SerializeField] public bool ControllerClickable = true;

        [Tooltip("If UseController is at false, will automatically be set at false.\n" +
            "If true, slider will fill only when the user is clicking on it.\n" +
            "If false, slider will fill only when the user is pointing at it.")]
        [SerializeField] public bool FillWithClick;

        [Tooltip("The time it takes to fill the slider.")]
        [SerializeField] public float FillTime;

        [Header("Whether the value should go down when user is not clicking")]
        [SerializeField] public bool ValueIsGoingDown = true;

        [SerializeField] public bool ResetFillOnRelease = true;

        [Header("Unity Events for bar filled and released.")]
        [SerializeField] public UnityEvent OnBarFilled;
        [Tooltip("The OnBarReleased will only be called if the bar was filled before the user release it.")]
        [SerializeField] public UnityEvent OnBarReleased;

        /// <summary>
        /// Used to determine how much of the bar should be filled.
        /// </summary>
        [HideInInspector] public float Timer;
        #endregion


        #region PRIVATE_VARIABLES
        private bool _barFilled;                                           // Whether the bar is currently filled.
        private Coroutine _fillBarRoutine;                                 // Reference to the coroutine that controls the bar filling up, used to stop it if required.

        /// <summary>
        /// Reference to the origin of the ray that is filling the slider
        /// </summary>
        private ERayOrigin _handFilling = ERayOrigin.NONE;

        private bool _boxColliderSetup;

        private bool _isFillingWithMesh;

        /// <summary>
        /// true when the events for ObjectWasClicked or Hovered were registered.
        /// </summary>
        private bool _eventWereRegistered;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                _boxColliderSetup = false;
                _eventWereRegistered = false;

                GetFillRectReference();

                if (LaserClickable)
                {
                    ObjectWasClickedEvent.Listeners += CheckSliderClick;
                    ObjectWasHoveredEvent.Listeners += CheckSliderHovered;
                    _eventWereRegistered = true;
                }

                if (ControllerClickable)
                    GetComponent<BoxCollider>().isTrigger = true;

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                    StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_eventWereRegistered)
            {
                ObjectWasClickedEvent.Listeners -= CheckSliderClick;
                ObjectWasHoveredEvent.Listeners -= CheckSliderHovered;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Application.isPlaying && _boxColliderSetup)
            {
                // if the bar is being filled
                if (!_isFillingWithMesh)
                {
                    if (_fillBarRoutine != null)
                    {
                        CheckHandStillOver();
                    }
                    else if (ValueIsGoingDown && value > 0)
                    {
                        // Set the value of the slider or the UV based on the normalised time.
                        Timer -= Time.deltaTime;
                        value = Timer / FillTime;
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ControllerClickable && interactable && other.gameObject.tag.Contains("ControllerBody"))
            {
                _isFillingWithMesh = true;
                HandleHandInteracting(other.gameObject.name.Contains("RIGHT") ? ERayOrigin.RIGHT_HAND : ERayOrigin.LEFT_HAND);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (ControllerClickable && interactable && other.gameObject.tag.Contains("ControllerBody"))
                HandleUp();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        private void CheckSliderClick(ObjectWasClickedEvent clickEvent)
        {
            if (IsInteractable() && FillWithClick)
            {
                // if the object clicked correspond to this transform and the coroutine to fill the bar didn't started yet
                if (clickEvent.ObjectClicked == transform && _fillBarRoutine == null)
                {
                    HandleHandInteracting(clickEvent.RayOrigin);
                }
                // If the user was clicking the bar but stopped
                else if (_fillBarRoutine != null)
                {
                    HandleUp();
                }
            }
        }

        /// <summary>
        /// Event called when the user is looking or pointing at the Slider
        /// </summary>
        /// <param name="hoverEvent">The event raised when an object is hovered</param>
        private void CheckSliderHovered(ObjectWasHoveredEvent hoverEvent)
        {
            if (IsInteractable() && !FillWithClick)
            {
                // if the object hovered correspond to this transform and the coroutine to fill the bar didn't started yet
                if (hoverEvent.ObjectHovered == transform && _fillBarRoutine == null)
                {
                    HandleHandInteracting(hoverEvent.RaycastOrigin);
                }
                // If the user was hovering the bar but stopped
                else if (_fillBarRoutine != null && hoverEvent.RaycastOrigin == _handFilling && hoverEvent.ObjectHovered != transform)
                {
                    HandleUp();
                }
            }
        }

        /// <summary>
        /// Check which hand is pointing toward the slider
        /// </summary>
        private void HandleHandInteracting(ERayOrigin handPointing)
        {
            _handFilling = handPointing;

            if (_handFilling != ERayOrigin.NONE && _fillBarRoutine == null)
                _fillBarRoutine = StartCoroutine(FillBar());
        }

        /// <summary>
        /// Coroutine called to fill the bar. Stop only if the user release it.
        /// </summary>
        /// <returns>a new IEnumerator</returns>
        private IEnumerator FillBar()
        {
            // Until the timer is greater than the fill time...
            while (Timer < FillTime)
            {
                // ... add to the timer the difference between frames.
                Timer += Time.deltaTime;

                // Set the value of the slider or the UV based on the normalised time.
                value = (Timer / FillTime);

                onValueChanged.Invoke(value);

                // Wait until next frame.
                yield return new WaitForEndOfFrame();

                // If the user is still looking at the bar, go on to the next iteration of the loop.
                if (_handFilling == ERayOrigin.LEFT_HAND || _handFilling == ERayOrigin.LEFT_HAND || _handFilling == ERayOrigin.CAMERA)
                    continue;

                // If the user is no longer looking at the bar, reset the timer and bar and leave the function.
                value = 0f;
                yield break;
            }
            // If the loop has finished the bar is now full.
            _barFilled = true;
            OnBarFilled.Invoke();
        }

        /// <summary>
        /// Method called when the user release the slider bar
        /// </summary>
        private void HandleUp()
        {
            // If the bar was filled and the user is releasing it, we invoke the OnBarReleased event
            if (_barFilled)
            {
                _barFilled = false;
                OnBarReleased?.Invoke();
            }

            // If the coroutine has been started (and thus we have a reference to it) stop it.
            if (_fillBarRoutine != null)
            {
                StopCoroutine(_fillBarRoutine);
                _fillBarRoutine = null;
            }

            // Reset the timer and bar values.
            if (!ValueIsGoingDown && ResetFillOnRelease)
            {
                Timer = 0f;
                value = 0.0f;
            }

            // Set the Hand filling at null
            _handFilling = ERayOrigin.NONE;
            _isFillingWithMesh = false;
        }

        /// <summary>
        /// Check if the Controller or the Gaze filling the bar is still over the Slider or, if we use the click, if the user is still clicking
        /// </summary>
        private void CheckHandStillOver()
        {
            switch (_handFilling)
            {
                // if we fill with click and the user is not clicking anymore
                // OR, if the user is not on the slider anymore

                case ERayOrigin.LEFT_HAND:
                    if (FillWithClick && !InteractionVariableContainer.IsClickingSomethingLeft)
                        HandleUp();
                    break;

                case ERayOrigin.RIGHT_HAND:
                    if (FillWithClick && !InteractionVariableContainer.IsClickingSomethingRight)
                        HandleUp();
                    break;

                case ERayOrigin.CAMERA:
                    if (FillWithClick && !InteractionVariableContainer.IsClickingSomethingGaze)
                        HandleUp();
                    break;
            }
        }


        /// <summary>
        /// Set the BoxCollider if the SetColliderAuto is at true
        /// </summary>
        /// <returns></returns>
        private IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();

            if (SetColliderAuto)
                VRUIBoxColliderSetup.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());
            
            _boxColliderSetup = true;
        }

        /// <summary>
        /// Try to get and set the fillRect reference by looking for a Fill object in the deepChildren
        /// </summary>
        private void GetFillRectReference()
        {
            try
            {
                fillRect = transform.FindDeepChild("Fill").GetComponent<RectTransform>();
            }
            catch
            {
                Debug.LogError("<b>[VRSF] :</b> Please add a \"Fill\" GameObject with RectTransform as a child or DeepChild of this VR Auto Fill Slider.");
            }
        }
        #endregion
    }
}