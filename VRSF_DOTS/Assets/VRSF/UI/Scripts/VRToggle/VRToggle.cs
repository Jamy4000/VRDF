using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VRSF.Core.Events;
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

        private bool _isSelected;
        #endregion VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                OnSetupVRReady.RegisterSetupVRResponse(Init);

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                    StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (OnSetupVRReady.IsMethodAlreadyRegistered(Init))
                OnSetupVRReady.Listeners -= Init;

            if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckToggleClick))
            {
                ObjectWasHoveredEvent.Listeners -= CheckObjectOvered;
                ObjectWasClickedEvent.Listeners -= CheckToggleClick;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ControllerClickable && interactable && other.gameObject.tag.Contains("ControllerBody"))
                isOn = !isOn;
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Event called when the button is clicked
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        private void CheckToggleClick(ObjectWasClickedEvent clickEvent)
        {
            if (interactable && clickEvent.ObjectClicked == transform)
                isOn = !isOn;
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
            if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
            {
                if (LaserClickable)
                {
                    ObjectWasClickedEvent.Listeners += CheckToggleClick;
                    ObjectWasHoveredEvent.Listeners += CheckObjectOvered;
                }

            if (ControllerClickable)
                    GetComponent<BoxCollider>().isTrigger = true;
            }
        }
        #endregion PRIVATE_METHODS
    }
}