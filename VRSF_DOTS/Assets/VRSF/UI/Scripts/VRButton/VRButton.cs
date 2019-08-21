using UnityEngine;
using System.Collections.Generic;
using VRSF.Core.Events;
using VRSF.Core.SetupVR;
using UnityEngine.EventSystems;
using VRSF.Core.Controllers.Haptic;
using VRSF.Core.Controllers;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRButton based on the Button for Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRButton : UnityEngine.UI.Button
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

            if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckObjectClicked))
            {
                ObjectWasHoveredEvent.Listeners -= CheckObjectOvered;
                ObjectWasClickedEvent.Listeners -= CheckObjectClicked;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ControllerClickable && interactable && other.gameObject.tag.Contains("ControllerBody"))
            {
                onClick.Invoke();
                new OnHapticRequestedEvent(other.name.ToLower().Contains("left") ? EHand.LEFT : EHand.RIGHT, 0.2f, 0.1f);
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Event called when the button is clicked
        /// </summary>
        /// <param name="objectClickEvent">The object that was clicked</param>
        void CheckObjectClicked(ObjectWasClickedEvent objectClickEvent)
        {
            if (interactable && objectClickEvent.ObjectClicked == transform)
                onClick.Invoke();
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
        IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            var boxCollider = GetComponent<BoxCollider>();
            var rectTrans = GetComponent<RectTransform>();
            if (boxCollider != null && rectTrans != null)
                VRUIBoxColliderSetup.CheckBoxColliderSize(boxCollider, rectTrans);
        }

        private void Init(OnSetupVRReady _)
        {
            if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
            {
                if (LaserClickable)
                {
                    ObjectWasClickedEvent.Listeners += CheckObjectClicked;
                    ObjectWasHoveredEvent.Listeners += CheckObjectOvered;
                }

                var boxCollider = GetComponent<BoxCollider>();
                if (ControllerClickable && boxCollider != null)
                    boxCollider.isTrigger = true;
            }
        }
        #endregion PRIVATE_METHODS
    }
}