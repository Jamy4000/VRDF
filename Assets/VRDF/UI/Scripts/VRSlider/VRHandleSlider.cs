using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRDF.Core.VRInteractions;
using VRDF.Core.Raycast;

namespace VRDF.UI
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
                SliderSetup();
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
        }

        protected override void Update()
        {
            base.Update();
            if (Application.isPlaying && interactable && _rayHoldingHandle != ERayOrigin.NONE)
                value = _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.GetCurrentHitPosition(_rayHoldingHandle));
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            VRRaycastAuthoring.CheckSceneContainsRaycaster();
        }
#endif
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
            VRUISetupHelper.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());
        }

        private void SliderSetup()
        {
            OnSetupVRReady.RegisterSetupVRCallback(CheckDevice);

            CheckSliderReferences();

            _scrollableSetup = new VRUIScrollableSetup(UnityUIToVRDFUI.SliderDirectionToUIDirection(direction), minValue, maxValue, wholeNumbers);
            _scrollableSetup.CheckMinMaxGameObjects(handleRect.parent, UnityUIToVRDFUI.SliderDirectionToUIDirection(direction), ref _minPosBar, ref _maxPosBar);
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            if (OnSetupVRReady.IsCallbackRegistered(CheckDevice))
                OnSetupVRReady.Listeners -= CheckDevice;

            if (VRDF_Components.DeviceLoaded != Core.SetupVR.EDevice.SIMULATOR || VRUISetupHelper.ShouldRegisterForSimulator(this))
            {
                OnVRClickerStartClicking.Listeners += CheckClickedObject;
                OnVRClickerStopClicking.Listeners += CheckUnclickedObject;
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
                Debug.LogError("<b>[VRDF] :</b> Please specify a HandleRect in the inspector as a child of this VR Handle Slider.", gameObject);
            }
        }
        #endregion
    }
}