﻿using System.Collections.Generic;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRToggle based on the Toggle for Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRToggle : UnityEngine.UI.Toggle
    {
        #region VARIABLES
        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;

        [Tooltip("If this button can be click using a Raycast and the trigger of your controller.")]
        [SerializeField] public bool LaserClickable = true;

        [Tooltip("If this button can be click using the meshcollider of your controller.")]
        [SerializeField] public bool ControllerClickable = true;

        [Tooltip("Event raised when hovering the button with your gaze or one of the controller's laser.")]
        [SerializeField] public UnityEngine.Events.UnityEvent OnHover = new UnityEngine.Events.UnityEvent();

        [Tooltip("Event raised when you stop hovering the button with your gaze or one of the controller's laser.")]
        [SerializeField] public UnityEngine.Events.UnityEvent OnStopHovering = new UnityEngine.Events.UnityEvent();

        private bool _isSelected;
        #endregion VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                OnSetupVRReady.RegisterSetupVRCallback(Init);

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                    StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetupVRReady.UnregisterSetupVRCallback(Init);

            if (OnObjectIsBeingHovered.IsMethodAlreadyRegistered(CheckObjectOvered))
                OnObjectIsBeingHovered.Listeners -= CheckObjectOvered;

            if (OnVRClickerIsClicking.IsMethodAlreadyRegistered(CheckObjectClick))
                OnVRClickerIsClicking.Listeners -= CheckObjectClick;
        }

        private void OnTriggerEnter(Collider other)
        {
            // if the user is in VR
            if (UnityEngine.XR.XRSettings.enabled && VRSF_Components.SetupVRIsReady)
            {
                var objectTag = other.gameObject.tag;
                if (ControllerClickable && interactable && (objectTag.Contains("ControllerBody") || objectTag.Contains("UIClicker")))
                {
                    isOn = !isOn;
                    new OnHapticRequestedEvent(other.name.ToLower().Contains("left") ? EHand.LEFT : EHand.RIGHT, 0.2f, 0.1f);
                }
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Event called when the button is clicked
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        private void CheckObjectClick(OnVRClickerIsClicking clickEvent)
        {
            if (interactable && clickEvent.ClickedObject == gameObject)
            {
                isOn = !isOn;
                new OnHapticRequestedEvent(clickEvent.RaycastOrigin == Core.Raycast.ERayOrigin.LEFT_HAND ? EHand.LEFT : EHand.RIGHT, 0.2f, 0.1f);
            }
        }

        private void CheckObjectOvered(OnObjectIsBeingHovered info)
        {
            if (info.HoveredObject == gameObject && interactable && !_isSelected)
            {
                _isSelected = true;
                new OnHapticRequestedEvent(info.RaycastOrigin == Core.Raycast.ERayOrigin.LEFT_HAND ? EHand.LEFT : EHand.RIGHT, 0.1f, 0.075f);
                OnHover.Invoke();
            }
            else if (info.HoveredObject != gameObject && _isSelected)
            {
                _isSelected = false;
                OnDeselect(null);
                OnStopHovering.Invoke();
            }
        }

        /// <summary>
        /// Setup the BoxCOllider size and center by colling the NotScrollableSetup method CheckBoxColliderSize.
        /// We use a coroutine and wait for the end of the first frame as the element cannot be correctly setup on the first frame
        /// </summary>
        /// <returns></returns>
        private IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            VRUIBoxColliderSetup.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());
        }

        private void Init(OnSetupVRReady _)
        {
            if (LaserClickable && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
            {
                OnObjectIsBeingHovered.Listeners += CheckObjectOvered;
                OnVRClickerIsClicking.Listeners += CheckObjectClick;
            }

            if (ControllerClickable)
                GetComponent<BoxCollider>().isTrigger = true;
        }
        #endregion PRIVATE_METHODS
    }
}