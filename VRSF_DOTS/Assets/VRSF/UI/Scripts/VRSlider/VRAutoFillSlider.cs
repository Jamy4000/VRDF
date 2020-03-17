using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
        #endregion


        #region PRIVATE_VARIABLES
        /// <summary>
        /// Whether the bar is currently filled.
        /// </summary>
        private bool _barFilled;
        
        /// <summary>
        /// Are we filling the bar right now ?
        /// </summary>
        private bool _isFilling;

        /// <summary>
        /// Reference to the origin of the ray that is filling the slider
        /// </summary>
        private ERayOrigin _handFilling = ERayOrigin.NONE;

        /// <summary>
        /// Are we filling the bar with the controller's mesh ?
        /// </summary>
        private bool _isFillingWithMesh;

        /// <summary>
        /// Used to determine how much of the bar should be filled.
        /// </summary>
        private float _timer;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                if (!VRRaycastAuthoring.SceneContainsRaycaster())
                    OnVRRaycasterIsSetup.Listeners += SetupAutoFillSlider;
                else
                    SetupAutoFillSlider(null);
            }
        }

        protected override void Start()
        {
            base.Start();
            // We setup the BoxCollider size and center
            if (Application.isPlaying && SetColliderAuto)
                StartCoroutine(SetupBoxCollider());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (OnVRClickerStartClicking.IsCallbackRegistered(CheckClickedObject))
            {
                OnVRClickerStartClicking.Listeners -= CheckClickedObject;
                OnVRClickerStopClicking.Listeners -= CheckUnclickedObject;
            }
            else if (OnStartHoveringObject.IsCallbackRegistered(CheckHoveredObject))
            {
                OnStartHoveringObject.Listeners -= CheckHoveredObject;
                OnStopHoveringObject.Listeners -= CheckUnhoveredObject;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Application.isPlaying)
            {
                if (_isFilling)
                    FillBar();
                else
                    CheckValueGoingDown();

#if UNITY_IOS || UNITY_ANDROID
                Check2DInputs();
#endif
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!ControllerClickable || !interactable)
                return;

            if (other.gameObject.CompareTag("UIClicker") || other.gameObject.CompareTag("ControllerBody"))
            {
                _isFillingWithMesh = true;
                HandleHandInteracting(other.gameObject.name.ToLower().Contains("right") ? ERayOrigin.RIGHT_HAND : ERayOrigin.LEFT_HAND);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isFillingWithMesh && other.gameObject.tag.Contains("ControllerBody"))
                HandleUp();
        }
#endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// 
        /// </summary>
        private void CheckValueGoingDown()
        {
            if (ValueIsGoingDown && value > 0)
            {
                // Set the value of the slider or the UV based on the normalised time.
                _timer -= Time.deltaTime;
                value = _timer / FillTime;
            }
        }

#if UNITY_IOS || UNITY_ANDROID
        /// <summary>
        /// Support for Mobile Platforms
        /// </summary>
        private void Check2DInputs()
        {
            if (Input.touchCount == 1)
            {
                var screenRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(screenRay, out RaycastHit hit, 200, ~LayerMask.NameToLayer("UI"), QueryTriggerInteraction.UseGlobal))
                    CheckCanFillSlider(hit.collider.gameObject, ERayOrigin.CAMERA);
            }
            else if (Input.touchCount == 0 && _handFilling != ERayOrigin.NONE)
            {
                HandleUp();
            }
        }
#endif

        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="startClickingEvent">The event raised when an object is clicked</param>
        private void CheckClickedObject(OnVRClickerStartClicking startClickingEvent)
        {
            CheckCanFillSlider(startClickingEvent.ClickedObject, startClickingEvent.RaycastOrigin);
        }

        /// <summary>
        /// Event called when the user stop clicking on something
        /// </summary>
        /// <param name="stopClickingEvent">The event raised when an object was unclicked</param>
        private void CheckUnclickedObject(OnVRClickerStopClicking stopClickingEvent)
        {
            CheckStopFillingSlider(stopClickingEvent.RaycastOrigin);
        }

        /// <summary>
        /// Event called when the user is looking or pointing at the Slider
        /// </summary>
        /// <param name="hoverEvent">The event raised when an object is hovered</param>
        private void CheckHoveredObject(OnStartHoveringObject hoverEvent)
        {
            CheckCanFillSlider(hoverEvent.HoveredObject, hoverEvent.RaycastOrigin);
        }

        /// <summary>
        /// Event called when the user stop looking or pointing at the Slider
        /// </summary>
        /// <param name="unhoverEvent">The event raised when an object is unhovered</param>
        private void CheckUnhoveredObject(OnStopHoveringObject unhoverEvent)
        {
            CheckStopFillingSlider(unhoverEvent.RaycastOrigin);
        }

        private void CheckCanFillSlider(GameObject toCheck, ERayOrigin raycastOrigin)
        {
            if (interactable && toCheck == gameObject)
                HandleHandInteracting(raycastOrigin);
        }

        private void CheckStopFillingSlider(ERayOrigin rayOrigin)
        {
            if (_isFilling && rayOrigin == _handFilling)
                HandleUp();
        }

        /// <summary>
        /// Check which hand is pointing toward the slider
        /// </summary>
        private void HandleHandInteracting(ERayOrigin handPointing)
        {
            if (_isFilling)
                return;

            _handFilling = handPointing;
            _isFilling = true;
            UIHapticGenerator.CreateClickHapticSignal(handPointing);
        }

        /// <summary>
        /// Coroutine called to fill the bar. Stop only if the user release it.
        /// </summary>
        /// <returns>a new IEnumerator</returns>
        private void FillBar()
        {
            if (_barFilled)
                return;

            // Until the timer is greater than the fill time...
            if (_timer < FillTime)
            {
                // ... add to the timer the difference between frames.
                _timer += Time.deltaTime;

                // Set the value of the slider or the UV based on the normalised time.
                normalizedValue = (_timer / FillTime);
            }
            else
            {
                // If the loop has finished the bar is now full.
                _barFilled = true;
                OnBarFilled.Invoke();
            }
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

            // Reset the timer and bar values.
            if (!ValueIsGoingDown && ResetFillOnRelease)
            {
                _timer = 0f;
                normalizedValue = 0.0f;
            }

            // Set the Hand filling at null
            _handFilling = ERayOrigin.NONE;
            _isFillingWithMesh = false;
            _isFilling = false;
        }

        /// <summary>
        /// Set the BoxCollider if the SetColliderAuto is at true
        /// </summary>
        /// <returns></returns>
        private IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();
            VRUISetupHelper.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());
        }

        private void SetupAutoFillSlider(OnVRRaycasterIsSetup _)
        {
            if (OnVRRaycasterIsSetup.IsCallbackRegistered(SetupAutoFillSlider))
                OnVRRaycasterIsSetup.Listeners -= SetupAutoFillSlider;

            if (fillRect == null)
                GetFillRectReference();

            if (LaserClickable)
            {
                if (FillWithClick)
                {
                    OnVRClickerStartClicking.Listeners += CheckClickedObject;
                    OnVRClickerStopClicking.Listeners += CheckUnclickedObject;
                }
                else
                {
                    OnStartHoveringObject.Listeners += CheckHoveredObject;
                    OnStopHoveringObject.Listeners += CheckUnhoveredObject;
                }
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