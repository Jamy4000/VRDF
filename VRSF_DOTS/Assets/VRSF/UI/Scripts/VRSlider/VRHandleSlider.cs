using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.VRInteractions;
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

        private VRUIScrollableSetup _scrollableSetup;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                if (!VRRaycastAuthoring.SceneContainsRaycaster())
                    OnVRRaycasterIsSetup.Listeners += SliderSetup;
                else
                    SliderSetup(null);
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
            OnVRClickerStartClicking.Listeners -= CheckClickedObject;
            OnVRClickerStopClicking.Listeners -= CheckUnclickedObject;
        }

        protected override void Update()
        {
            base.Update();
            if (Application.isPlaying && interactable)
            {
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
        /// <param name="startClickingEvent">The event raised when an object was just clicked</param>
        private void CheckClickedObject(OnVRClickerStartClicking startClickingEvent)
        {
            if (CanHoldHandle())
            {
                _rayHoldingHandle = startClickingEvent.RaycastOrigin;
                UIHapticGenerator.CreateClickHapticSignal(_rayHoldingHandle);
            }

            bool CanHoldHandle()
            {
                return interactable && startClickingEvent.ClickedObject == gameObject && _rayHoldingHandle == ERayOrigin.NONE;
            }
        }

        private void CheckUnclickedObject(OnVRClickerStopClicking stopClickingEvent)
        {
            if (stopClickingEvent.UnclickedObject == gameObject)
                _rayHoldingHandle = ERayOrigin.NONE;
        }

        /// <summary>
        /// Set the BoxCollider size if SetColliderAuto is at true
        /// </summary>
        /// <returns></returns>
        private IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();
            VRUIBoxColliderSetup.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());
        }

        private void SliderSetup(OnVRRaycasterIsSetup _)
        {
            OnVRRaycasterIsSetup.Listeners -= SliderSetup;

            OnVRClickerStartClicking.Listeners += CheckClickedObject;
            OnVRClickerStopClicking.Listeners += CheckUnclickedObject;

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
        #endregion
    }
}