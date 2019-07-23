using System.Collections.Generic;
using VRSF.Core.Utils;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.VRInteractions;
using VRSF.Core.Events;
using VRSF.Core.Raycast;

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

        private Dictionary<ERayOrigin, Transform> _hitTransformDictionary;

        private IUISetupScrollable _scrollableSetup;

        private bool _boxColliderSetup;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                _boxColliderSetup = false;

                SetupUIElement();

                // We setup the BoxCollider size and center
                StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckSliderClick))
                ObjectWasClickedEvent.Listeners -= CheckSliderClick;
        }

        protected override void Update()
        {
            base.Update();
            if (Application.isPlaying && _boxColliderSetup)
            {
                CheckClickDown();

                if (_rayHoldingHandle != ERayOrigin.NONE)
                    value = _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, _hitTransformDictionary[_rayHoldingHandle].position);
            }
        }
        #endregion


        #region PRIVATE_METHODS
        private void SetupUIElement()
        {
            _scrollableSetup = new VRUIScrollableSetup(UnityUIToVRSFUI.SliderDirectionToUIDirection(direction), minValue, maxValue, wholeNumbers);

            CheckSliderReferences();
            
            ObjectWasClickedEvent.Listeners += CheckSliderClick;

            _hitTransformDictionary = new Dictionary<ERayOrigin, Transform>
            {
                { ERayOrigin.RIGHT_HAND, InteractionVariableContainer.PreviousRightHit },
                { ERayOrigin.LEFT_HAND, InteractionVariableContainer.PreviousLeftHit },
                { ERayOrigin.CAMERA, InteractionVariableContainer.PreviousGazeHit }
            };

            _scrollableSetup.CheckMinMaxGameObjects(handleRect.parent, UnityUIToVRSFUI.SliderDirectionToUIDirection(direction));

            _scrollableSetup.SetMinMaxPos(ref _minPosBar, ref _maxPosBar, handleRect.parent);
        }


        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        private void CheckSliderClick(ObjectWasClickedEvent clickEvent)
        {
            if (interactable && clickEvent.ObjectClicked == transform && _rayHoldingHandle == ERayOrigin.NONE)
            {
                _rayHoldingHandle = clickEvent.RayOrigin;
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
        #endregion
    }
}