﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using VRSF.Core.Controllers;
using VRSF.Core.VRInteractions;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new ScrollBar based on the Unity scrollbar, but adapted for VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider), typeof(Image))]
    public class VRScrollBar : Scrollbar
    {
        #region PUBLIC_VARIABLES
        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        #endregion


        #region PRIVATE_VARIABLES
        private Transform _minPosBar;
        private Transform _maxPosBar;

        private ERayOrigin _rayHoldingHandle = ERayOrigin.NONE;

        private VRUIScrollableSetup _scrollableSetup;

        private bool _boxColliderSetup;

        private bool _isSelected;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                OnSetupVRReady.RegisterSetupVRCallback(Init);

                // We setup the BoxCollider size and center
                StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetupVRReady.UnregisterSetupVRCallback(Init);

            if (OnVRClickerIsClicking.IsMethodAlreadyRegistered(CheckObjectClick))
                OnVRClickerIsClicking.Listeners -= CheckObjectClick;

            if (OnObjectIsBeingHovered.IsMethodAlreadyRegistered(CheckObjectOvered))
                OnObjectIsBeingHovered.Listeners -= CheckObjectOvered;
        }

        protected override void Update()
        {
            base.Update();
            if (Application.isPlaying && _boxColliderSetup)
            {
                CheckClickDown();

                switch (_rayHoldingHandle)
                {
                    case ERayOrigin.LEFT_HAND:
                        value = _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentLeftHitPosition);
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        value = _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentRightHitPosition);
                        break;
                    case ERayOrigin.CAMERA:
                        value = _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentGazeHitPosition);
                        break;
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        private void CheckObjectClick(OnVRClickerIsClicking clickEvent)
        {
            if (interactable && clickEvent.ClickedObject == gameObject && _rayHoldingHandle == ERayOrigin.NONE)
            {
                _rayHoldingHandle = clickEvent.RaycastOrigin;
                new OnHapticRequestedEvent(_rayHoldingHandle == ERayOrigin.LEFT_HAND ? EHand.LEFT : EHand.RIGHT, 0.2f, 0.1f);
            }
        }

        private void CheckObjectOvered(OnObjectIsBeingHovered info)
        {
            if (info.HoveredObject == gameObject && interactable && !_isSelected)
            {
                _isSelected = true;
                new OnHapticRequestedEvent(info.RaycastOrigin == ERayOrigin.LEFT_HAND ? EHand.LEFT : EHand.RIGHT, 0.1f, 0.075f);
            }
            else if (info.HoveredObject != gameObject && _isSelected)
            {
                _isSelected = false;
                OnDeselect(null);
            }
        }

        /// <summary>
        /// Depending on the hand holding the trigger, call the CheckClickStillDown with the right boolean
        /// </summary>
        private void CheckClickDown()
        {
            switch (_rayHoldingHandle)
            {
                case ERayOrigin.CAMERA:
                    _scrollableSetup.CheckClickStillDown(ref _rayHoldingHandle, InteractionVariableContainer.IsClickingSomethingGaze);
                    break;
                case ERayOrigin.LEFT_HAND:
                    _scrollableSetup.CheckClickStillDown(ref _rayHoldingHandle, InteractionVariableContainer.IsClickingSomethingLeft);
                    break;
                case ERayOrigin.RIGHT_HAND:
                    _scrollableSetup.CheckClickStillDown(ref _rayHoldingHandle, InteractionVariableContainer.IsClickingSomethingRight);
                    break;
            }
        }

        /// <summary>
        /// Start a coroutine that wait for the second frame to set the BoxCollider
        /// </summary>
        /// <returns></returns>
        private IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (SetColliderAuto)
                VRUIBoxColliderSetup.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());

            _boxColliderSetup = true;
        }

        /// <summary>
        /// Try to get the handleRect rectTransform reference by finding the Handle deepChild of this GameObject
        /// </summary>
        private void GetHandleRectReference()
        {
            try
            {
                if (handleRect == null)
                    handleRect = transform.FindDeepChild("Handle").GetComponent<RectTransform>();
            }
            catch
            {
                Debug.LogError("<b>[VRSF] :</b> Please specify a HandleRect in the inspector as a child of this VR Handle Slider.", gameObject);
            }
        }

        private void Init(OnSetupVRReady _)
        {
            GetHandleRectReference();

            // We register the Listener
            if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
            {
                OnObjectIsBeingHovered.Listeners += CheckObjectOvered;
                OnVRClickerIsClicking.Listeners += CheckObjectClick;
            }

            _scrollableSetup = new VRUIScrollableSetup(UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(direction));
            // Check if the Min and Max object are already created, and set there references
            _scrollableSetup.CheckMinMaxGameObjects(handleRect.parent, UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(direction), ref _minPosBar, ref _maxPosBar);

            value = 1;
        }
        #endregion
    }
}