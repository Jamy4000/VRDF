using VRSF.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRSF.Core.VRInteractions;
using VRSF.Core.Events;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.UI
{
    /// <summary>
    /// Handle the references and setup for the GameEvents, GameEventListeners and boxCollider of the VRAutoFillSlider
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRAutoFillSlider : Slider
    {
        #region PUBLIC_VARIABLES
        /// <summary>
        /// If you want to set the collider yourself, set this value to false.
        /// </summary>
        [SerializeField] [HideInInspector] public bool SetColliderAuto = true;

        /// <summary>
        /// If this slider can be click using a Raycast and the trigger of your controller.
        /// </summary>
        [SerializeField] [HideInInspector] public bool LaserClickable = true;

        /// <summary>
        /// If this slider can be click using the meshcollider of your controller.
        /// </summary>
        [SerializeField] [HideInInspector] public bool ControllerClickable = true;

        /// <summary>
        /// If UseController is at false, will automatically be set at false.
        /// If true, slider will fill only when the user is clicking on it.
        /// If false, slider will fill only when the user is pointing at it.
        /// </summary>
        [SerializeField] [HideInInspector] public bool FillWithClick;

        /// <summary>
        /// The time it takes to fill the slider.
        /// </summary>
        [SerializeField] [HideInInspector] public float FillTime;

        /// <summary>
        /// Whether the value should go down when user is not clicking
        /// </summary>
        [SerializeField] [HideInInspector] public bool ValueIsGoingDown = true;

        [SerializeField] [HideInInspector] public bool ResetFillOnRelease = true;

        /// <summary>
        /// Unity Events for bar filled and released.
        /// </summary>
        [SerializeField] [HideInInspector] public UnityEvent OnBarFilled;

        /// <summary>
        /// The OnBarReleased will only be called if the bar was filled before the user release it
        /// </summary>
        [SerializeField] [HideInInspector] public UnityEvent OnBarReleased;

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
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
            {
                if (UnityEngine.XR.XRSettings.enabled)
                    OnSetupVRReady.RegisterSetupVRResponse(Init);

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                {
                    _boxColliderSetup = false;
                    StartCoroutine(SetupBoxCollider());
                }
                else
                {
                    _boxColliderSetup = true;
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (OnSetupVRReady.IsMethodAlreadyRegistered(Init))
                OnSetupVRReady.Listeners -= Init;

            if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckSliderClick))
                ObjectWasClickedEvent.Listeners -= CheckSliderClick;

            if (ObjectWasHoveredEvent.IsMethodAlreadyRegistered(CheckSliderHovered))
                ObjectWasHoveredEvent.Listeners -= CheckSliderHovered;
        }

        protected override void Update()
        {
            if (Application.isPlaying && interactable && _boxColliderSetup)
            {
                // if the bar is being filled
                if (UnityEngine.XR.XRSettings.enabled && VRSF_Components.SetupVRIsReady)
                {
                    base.Update();
                    CheckVRInputs();
                }
                // Support for 2D Users
                else
                {
                    Check2DInputs();
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

        private void CheckVRInputs()
        {
            if (_isFillingWithMesh)
                return;

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

        private void Check2DInputs()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount == 1)
            {
                var mouseRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
                if (Physics.Raycast(mouseRay, out RaycastHit hit, 200, ~LayerMask.NameToLayer("UI"), QueryTriggerInteraction.UseGlobal))
                    CheckTransform(hit.transform, ERayOrigin.CAMERA);
            }
#if UNITY_STANDALONE || UNITY_EDITOR
            else if (Input.GetMouseButtonUp(0))
            {
#elif UNITY_IOS || UNITY_ANDROID
            else if (Input.touchCount == 0 && _handFilling != ERayOrigin.NONE)
            {
#endif
                HandleUp();
            }
            else if (_handFilling == ERayOrigin.NONE && ValueIsGoingDown && value > 0)
            {
                // Set the value of the slider or the UV based on the normalised time.
                Timer -= Time.deltaTime;
                value = Timer / FillTime;
            }
        }

        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        private void CheckSliderClick(ObjectWasClickedEvent clickEvent)
        {
            CheckTransform(clickEvent.ObjectClicked, clickEvent.RayOrigin);
        }

        /// <summary>
        /// Event called when the user is looking or pointing at the Slider
        /// </summary>
        /// <param name="hoverEvent">The event raised when an object is hovered</param>
        private void CheckSliderHovered(ObjectWasHoveredEvent hoverEvent)
        {
            CheckTransform(hoverEvent.ObjectHovered, hoverEvent.RaycastOrigin);
        }

        private void CheckTransform(Transform toCheck, ERayOrigin raycastOrigin)
        {
            if (IsInteractable())
            {
                // if the object hovered correspond to this transform and the coroutine to fill the bar didn't started yet
                if (toCheck == transform && _fillBarRoutine == null)
                {
                    HandleHandInteracting(raycastOrigin);
                }
                // If the user was hovering the bar but stopped
                else if (_fillBarRoutine != null && raycastOrigin == _handFilling && toCheck != transform)
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

                // Wait until next frame.
                yield return new WaitForEndOfFrame();

                // If the user is still looking at the bar, go on to the next iteration of the loop.
                if (_handFilling == ERayOrigin.LEFT_HAND || _handFilling == ERayOrigin.RIGHT_HAND || _handFilling == ERayOrigin.CAMERA)
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

            VRUIBoxColliderSetup.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());

            _boxColliderSetup = true;
        }

        private void Init(OnSetupVRReady _)
        {
            GetFillRectReference();

            if (LaserClickable)
            {
                if (FillWithClick)
                    ObjectWasClickedEvent.Listeners += CheckSliderClick;
                else
                    ObjectWasHoveredEvent.Listeners += CheckSliderHovered;
            }

            if (ControllerClickable)
                GetComponent<BoxCollider>().isTrigger = true;
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