using UnityEngine;
using System.Collections.Generic;
using VRSF.Core.Events;
using VRSF.Core.SetupVR;
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

        [Tooltip("Event raised when hovering the button with your gaze or one of the controller's laser.")]
        [SerializeField] public UnityEngine.Events.UnityEvent OnHover = new UnityEngine.Events.UnityEvent();

        [Tooltip("Event raised when you stop hovering the button with your gaze or one of the controller's laser.")]
        [SerializeField] public UnityEngine.Events.UnityEvent OnStopHovering = new UnityEngine.Events.UnityEvent();

        private Core.Raycast.ERayOrigin _handHovering = Core.Raycast.ERayOrigin.NONE;
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

            if (ObjectWasHoveredEvent.IsMethodAlreadyRegistered(CheckObjectOvered))
                ObjectWasHoveredEvent.Listeners -= CheckObjectOvered;

            if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckObjectClicked))
                ObjectWasClickedEvent.Listeners -= CheckObjectClicked;
        }

        private void OnTriggerEnter(Collider other)
        {
            // if the user is in VR
            if (UnityEngine.XR.XRSettings.enabled && VRSF_Components.SetupVRIsReady)
            {
                var objectTag = other.gameObject.tag;
                if (ControllerClickable && interactable && (objectTag.Contains("ControllerBody") || objectTag.Contains("UIClicker")))
                {
                    onClick.Invoke();
                    new OnHapticRequestedEvent(other.name.ToLower().Contains("left") ? EHand.LEFT : EHand.RIGHT, 0.2f, 0.1f);
                }
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
            if (CheckTransform(objectClickEvent.ObjectClicked))
                onClick.Invoke();
        }

        private bool CheckTransform(Transform toCheck)
        {
            return interactable && toCheck == transform;
        }

        private void CheckObjectOvered(ObjectWasHoveredEvent info)
        {
            if (CheckTransform(info.ObjectHovered) && _handHovering == Core.Raycast.ERayOrigin.NONE)
            {
                _handHovering = info.RaycastOrigin;
                OnSelect(null);
                OnHover.Invoke();
            }
            else if (info.ObjectHovered != transform && _handHovering == info.RaycastOrigin)
            {
                _handHovering = Core.Raycast.ERayOrigin.NONE;
                OnDeselect(null);
                OnStopHovering.Invoke();
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
            if (LaserClickable && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
            {
                ObjectWasHoveredEvent.Listeners += CheckObjectOvered;
                ObjectWasClickedEvent.Listeners += CheckObjectClicked;
            }

            var boxCollider = GetComponent<BoxCollider>();
            if (ControllerClickable && boxCollider != null)
                boxCollider.isTrigger = true;
        }
#endregion PRIVATE_METHODS
    }
}