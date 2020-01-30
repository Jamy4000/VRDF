using System.Collections.Generic;
using VRSF.Core.Utils;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.VRInteractions;
using VRSF.Core.Events;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using System;

namespace VRSF.UI
{
    /// <summary>
    /// This type of slider let the user click on a slider handler and move it through the slider bar.
    /// It work like a normal slider, and can be use for parameters or other GameObject that needs the SLider fill value.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRHandleSlider : Slider
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
        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
            {
                _boxColliderSetup = false;

                if (!UnityEngine.XR.XRSettings.enabled || !VRSF_Components.SetupVRIsReady)
                    Init(null);
                else
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

            if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckObjectClick))
                ObjectWasClickedEvent.Listeners -= CheckObjectClick;

            if (ObjectWasHoveredEvent.IsMethodAlreadyRegistered(CheckObjectOvered))
                ObjectWasHoveredEvent.Listeners -= CheckObjectOvered;
        }

        protected override void Update()
        {
            base.Update();
            if (Application.isPlaying && interactable && _boxColliderSetup)
            {
                // Support for 2D Users
                if (!UnityEngine.XR.XRSettings.enabled || !VRSF_Components.SetupVRIsReady)
                    Check2DInputs();

                switch (_rayHoldingHandle)
                {
                    case ERayOrigin.LEFT_HAND:
                        _scrollableSetup.CheckClickStillDown(ref _rayHoldingHandle, InteractionVariableContainer.IsClickingSomethingLeft);
                        value = InteractionVariableContainer.CurrentLeftHit != transform ? value : _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentLeftHitPosition);
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        _scrollableSetup.CheckClickStillDown(ref _rayHoldingHandle, InteractionVariableContainer.IsClickingSomethingRight);
                        value = InteractionVariableContainer.CurrentRightHit != transform ? value : _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentRightHitPosition);
                        break;
                    case ERayOrigin.CAMERA:
                        _scrollableSetup.CheckClickStillDown(ref _rayHoldingHandle, InteractionVariableContainer.IsClickingSomethingGaze);
                        value = InteractionVariableContainer.CurrentGazeHit != transform ? value : _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentGazeHitPosition);
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
        private void CheckObjectClick(ObjectWasClickedEvent clickEvent)
        {
            CheckTransform(clickEvent.ObjectClicked, clickEvent.RayOrigin);
        }

        private void CheckTransform(Transform toCheck, ERayOrigin raycastOrigin)
        {
            _rayHoldingHandle = interactable && ObjectClickedIsThis() ? raycastOrigin : ERayOrigin.NONE;

            bool ObjectClickedIsThis()
            {
                return toCheck == transform && _rayHoldingHandle == ERayOrigin.NONE;
            }
        }

        private void CheckObjectOvered(ObjectWasHoveredEvent info)
        {
            if (info.ObjectHovered == transform && interactable && !_isSelected)
            {
                _isSelected = true;
                OnSelect(null);
            }
            else if (info.ObjectHovered != transform && _isSelected)
            {
                _isSelected = false;
            }
        }


        /// <summary>
        /// Set the BoxCollider size if SetColliderAuto is at true
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
            if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR && VRSF_Components.DeviceLoaded != EDevice.NONE)
            {
                ObjectWasHoveredEvent.Listeners += CheckObjectOvered;
                ObjectWasClickedEvent.Listeners += CheckObjectClick;
            }

            CheckSliderReferences();

            _scrollableSetup = new VRUIScrollableSetup(UnityUIToVRSFUI.SliderDirectionToUIDirection(direction), minValue, maxValue, wholeNumbers);
            _scrollableSetup.CheckMinMaxGameObjects(handleRect.parent, UnityUIToVRSFUI.SliderDirectionToUIDirection(direction), ref _minPosBar, ref _maxPosBar);
        }

        private void CheckSliderReferences()
        {
            try
            {
                handleRect = transform.FindDeepChild("Handle").GetComponent<RectTransform>();
                fillRect = transform.FindDeepChild("Fill").GetComponent<RectTransform>();
            }
            catch
            {
                Debug.LogError("<b>[VRSF] :</b> Please specify a HandleRect in the inspector as a child of this VR Handle Slider.", gameObject);
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
                {
                    CheckTransform(hit.transform, ERayOrigin.CAMERA);
                    if (_rayHoldingHandle == ERayOrigin.CAMERA)
                    {
                        InteractionVariableContainer.CurrentGazeHit = transform;
                        InteractionVariableContainer.IsClickingSomethingGaze = true;
                        InteractionVariableContainer.CurrentGazeHitPosition = hit.point;
                    }
                }
            }
#if UNITY_STANDALONE || UNITY_EDITOR
            else if (Input.GetMouseButtonUp(0) && _rayHoldingHandle != ERayOrigin.NONE)
            {
#elif UNITY_IOS || UNITY_ANDROID
            else if (Input.touchCount == 0 && _rayHoldingHandle != ERayOrigin.NONE)
            {
#endif
                _rayHoldingHandle = ERayOrigin.NONE;
                InteractionVariableContainer.IsClickingSomethingGaze = false;
                InteractionVariableContainer.CurrentGazeHit = null;
            }
#if UNITY_STANDALONE || UNITY_EDITOR
            else if (Input.GetMouseButton(0) && _rayHoldingHandle == ERayOrigin.CAMERA)
            {
                var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif UNITY_IOS || UNITY_ANDROID
            else if (Input.touchCount == 1 && _rayHoldingHandle == ERayOrigin.CAMERA)
            {
                var mouseRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
# endif
                if (Physics.Raycast(mouseRay, out RaycastHit hit, 200, ~LayerMask.NameToLayer("UI"), QueryTriggerInteraction.UseGlobal))
                    InteractionVariableContainer.CurrentGazeHitPosition = hit.point;
            }
        }
#endregion
    }
}