using UnityEngine;
using System.Collections.Generic;

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
        #endregion VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                if (!Core.Raycast.VRRaycastAuthoring.SceneContainsRaycaster())
                    Core.Raycast.OnVRRaycasterIsSetup.Listeners += SetupVRButton;
                else
                    SetupVRButton(null);
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
            if (OnStartHoveringObject.IsCallbackRegistered(CheckHoveredObject))
            {
                OnStartHoveringObject.Listeners -= CheckHoveredObject;
                OnStopHoveringObject.Listeners -= CheckUnhoveredObject;

                if (OnVRClickerStartClicking.IsCallbackRegistered(CheckClickedObject))
                    OnVRClickerStartClicking.Listeners -= CheckClickedObject;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var objectTag = other.gameObject.tag;
            if (ControllerClickable && interactable && (objectTag.Contains("ControllerBody") || objectTag.Contains("UIClicker")))
            {
                onClick.Invoke();
                UIHapticGenerator.CreateClickHapticSignal(other.name.ToLower().Contains("left") ? Core.Raycast.ERayOrigin.LEFT_HAND : Core.Raycast.ERayOrigin.RIGHT_HAND);
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="startClickingEvent">The event raised when an object was clicked</param>
        private void CheckClickedObject(OnVRClickerStartClicking startClickingEvent)
        {
            if (CheckGameObject(startClickingEvent.ClickedObject))
            {
                onClick.Invoke();
                UIHapticGenerator.CreateClickHapticSignal(startClickingEvent.RaycastOrigin);
            }
        }

        private void CheckHoveredObject(OnStartHoveringObject info)
        {
            if (CheckGameObject(info.HoveredObject))
                OnHover.Invoke();
        }

        private void CheckUnhoveredObject(OnStopHoveringObject info)
        {
            if (CheckGameObject(info.UnhoveredObject))
                OnStopHovering.Invoke();
        }

        private bool CheckGameObject(GameObject toCheck)
        {
            return interactable && toCheck == gameObject;
        }

        /// <summary>
        /// Setup the BoxCOllider size and center by colling the NotScrollableSetup method CheckBoxColliderSize.
        /// We use a coroutine and wait for the end of the first frame as the element cannot be correctly setup on the first frame
        /// </summary>
        /// <returns></returns>
        IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();

            var boxCollider = GetComponent<BoxCollider>();
            var rectTrans = GetComponent<RectTransform>();
            if (boxCollider != null && rectTrans != null)
                VRUISetupHelper.CheckBoxColliderSize(boxCollider, rectTrans);
        }

        private void SetupVRButton(Core.Raycast.OnVRRaycasterIsSetup _)
        {
            Core.Raycast.OnVRRaycasterIsSetup.Listeners -= SetupVRButton;

            if (LaserClickable)
            {
                OnStartHoveringObject.Listeners += CheckHoveredObject;
                OnStopHoveringObject.Listeners += CheckUnhoveredObject;
                
                if (VRUISetupHelper.ShouldRegisterForSimulator(this))
                    OnVRClickerStartClicking.Listeners += CheckClickedObject;
            }

            var boxCollider = GetComponent<BoxCollider>();
            if (ControllerClickable && boxCollider != null)
                boxCollider.isTrigger = true;
        }
        #endregion PRIVATE_METHODS
    }
}