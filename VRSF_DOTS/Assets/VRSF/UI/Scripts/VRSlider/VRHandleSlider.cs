using System.Collections.Generic;
using VRSF.Core.Utils;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.VRInteractions;
using VRSF.Core.Events;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using UnityEngine.EventSystems;

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
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                _boxColliderSetup = false;

                OnSetupVRReady.RegisterSetupVRResponse(Init);

                // We setup the BoxCollider size and center
                StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (OnSetupVRReady.IsMethodAlreadyRegistered(Init))
                OnSetupVRReady.Listeners -= Init;

            if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckSliderClick))
            {
                ObjectWasHoveredEvent.Listeners -= CheckObjectOvered;
                ObjectWasClickedEvent.Listeners -= CheckSliderClick;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (Application.isPlaying && _boxColliderSetup)
            {
                switch (_rayHoldingHandle)
                {
                    case ERayOrigin.LEFT_HAND:
                        _scrollableSetup.CheckClickStillDown(ref _rayHoldingHandle, InteractionVariableContainer.IsClickingSomethingLeft);
                        value = _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentLeftHitPosition);
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        _scrollableSetup.CheckClickStillDown(ref _rayHoldingHandle, InteractionVariableContainer.IsClickingSomethingRight);
                        value = _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentRightHitPosition);
                        break;
                    case ERayOrigin.CAMERA:
                        _scrollableSetup.CheckClickStillDown(ref _rayHoldingHandle, InteractionVariableContainer.IsClickingSomethingGaze);
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
        private void CheckSliderClick(ObjectWasClickedEvent clickEvent)
        {
            _rayHoldingHandle = interactable && ObjectClickedIsThis() ? clickEvent.RayOrigin : ERayOrigin.NONE;

            bool ObjectClickedIsThis()
            {
                return clickEvent.ObjectClicked == transform && _rayHoldingHandle == ERayOrigin.NONE;
            }
        }

        private void CheckObjectOvered(ObjectWasHoveredEvent info)
        {
            var currentEventSystem = EventSystem.current;
            if (info.ObjectHovered == transform && interactable && !_isSelected)
            {
                _isSelected = true;
                OnSelect(new BaseEventData(currentEventSystem));
            }
            else if (info.ObjectHovered != transform && _isSelected)
            {
                _isSelected = false;
                OnDeselect(new BaseEventData(currentEventSystem));
            }
        }


        /// <summary>
        /// Set the BoxCollider size if SetColliderAuto is at true
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

        private void Init(OnSetupVRReady _)
        {
            if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
            {
                ObjectWasClickedEvent.Listeners += CheckSliderClick;
                ObjectWasHoveredEvent.Listeners += CheckObjectOvered;
            
                CheckSliderReferences();

                _scrollableSetup = new VRUIScrollableSetup(UnityUIToVRSFUI.SliderDirectionToUIDirection(direction), minValue, maxValue, wholeNumbers);
                _scrollableSetup.CheckMinMaxGameObjects(handleRect.parent, UnityUIToVRSFUI.SliderDirectionToUIDirection(direction), ref _minPosBar, ref _maxPosBar);
            }
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
        #endregion
    }
}