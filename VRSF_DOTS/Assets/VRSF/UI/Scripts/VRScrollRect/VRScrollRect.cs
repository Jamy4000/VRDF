using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.VRInteractions;
using VRSF.Core.Events;
using VRSF.Core.Utils;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRScrollRect based on the ScrollRect for Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRScrollRect : ScrollRect
    {
        #region PUBLIC_VARIABLES
        [Header("The Direction of this ScrollRect.")]
        [Tooltip("Will be override by the Scrollbar direction if there's at least one.")]
        [SerializeField] public EUIDirection Direction = EUIDirection.TopToBottom;

        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        #endregion


        #region PRIVATE_VARIABLES
        private Transform _minPosBar;
        private Transform _maxPosBar;

        private ERayOrigin _rayHoldingHandle = ERayOrigin.NONE;

        private VRUIScrollableSetup _scrollableSetup;

        private bool _boxColliderSetup;

        /// <summary>
        /// true when the events for ObjectWasClicked or Hovered were registered.
        /// </summary>
        private bool _eventWereRegistered;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                _scrollableSetup = new VRUIScrollableSetup(Direction);

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

            if (_eventWereRegistered)
            {
                if (verticalScrollbar != null)
                    verticalScrollbar.onValueChanged.RemoveAllListeners();

                if (horizontalScrollbar != null)
                    horizontalScrollbar.onValueChanged.RemoveAllListeners();

                if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckObjectClick))
                    ObjectWasClickedEvent.Listeners -= CheckObjectClick;

                _eventWereRegistered = false;
            }
        }

        private void Update()
        {
            if (Application.isPlaying && _boxColliderSetup)
            {
                CheckClickDown();

                if (_rayHoldingHandle != ERayOrigin.NONE)
                {
                    if (vertical && verticalScrollbar)
                        verticalScrollbar.value = CheckScrollbarValue();

                    if (horizontal && horizontalScrollbar)
                        horizontalScrollbar.value = CheckScrollbarValue();
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        private float CheckScrollbarValue()
        {
            switch (_rayHoldingHandle)
            {
                case ERayOrigin.LEFT_HAND:
                    return _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentLeftHitPosition);
                case ERayOrigin.RIGHT_HAND:
                    return _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentRightHitPosition);
                case ERayOrigin.CAMERA:
                    return _scrollableSetup.SetComponentNewValue(_minPosBar.position, _maxPosBar.position, InteractionVariableContainer.CurrentGazeHitPosition);
                default:
                    // Never happening
                    return 0.0f;
            }
        }

        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="clickEvent">The event raised when something is clicked</param>
        void CheckObjectClick(ObjectWasClickedEvent clickEvent)
        {
            if (clickEvent.ObjectClicked == transform && _rayHoldingHandle == ERayOrigin.NONE)
                _rayHoldingHandle = clickEvent.RayOrigin;
        }

        /// <summary>
        /// Depending on the hand holding the trigger, call the CheckClickStillDown with the right boolean
        /// </summary>
        void CheckClickDown()
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
        IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (SetColliderAuto)
            {
                BoxCollider box = GetComponent<BoxCollider>();
                box = VRUIBoxColliderSetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
                box.center = Vector3.zero;

                if (vertical)
                {
                    var barCollider = verticalScrollbar.GetComponent<BoxCollider>();
                    float x = (box.size.x - barCollider.size.x);
                    box.size = new Vector3(x, box.size.y, box.size.z);
                    box.center = new Vector3(-barCollider.size.x / 2, box.center.y, box.center.z + 0.1f);
                }

                if (horizontal)
                {
                    var barCollider = horizontalScrollbar.GetComponent<BoxCollider>();
                    float y = (box.size.y - barCollider.size.y);
                    box.size = new Vector3(box.size.x, y, box.size.z);
                    box.center = new Vector3(box.center.x, barCollider.size.y / 2, box.center.z + 0.1f);
                }
            }

            _scrollableSetup.CheckContentStatus(viewport, content);
            _boxColliderSetup = true;
        }

        private void Init(OnSetupVRReady _)
        {
            // We setup the references to the ScrollRect elements
            SetScrollRectReferences();

            // We override the directio selected in the inspector by the scrollbar direction if we use one
            // The vertical direction will always have top priority on the horizontal direction
            if (vertical && verticalScrollbar != null)
            {
                Direction = UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(verticalScrollbar.direction);
                verticalScrollbar.onValueChanged.AddListener(delegate { OnValueChangedCallback(); });
            }
            else if (horizontal && horizontalScrollbar != null)
            {
                Direction = UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(horizontalScrollbar.direction);
                horizontalScrollbar.onValueChanged.AddListener(delegate { OnValueChangedCallback(); });
            }

            if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
                ObjectWasClickedEvent.Listeners += CheckObjectClick;

            _eventWereRegistered = true;

            // We setup the Min and Max pos transform
            _scrollableSetup.CheckMinMaxGameObjects(transform, Direction, ref _minPosBar, ref _maxPosBar);
        }

        /// <summary>
        /// Set the scrollRect references (scrollbar, content and viewport) by looking in the DeepChildren
        /// </summary>
        void SetScrollRectReferences()
        {
            // get VRScrollBarVertical if we use the scrollrect as vertical
            try
            {
                if (vertical && verticalScrollbar == null)
                    verticalScrollbar = transform.FindDeepChild("VRScrollbarVertical").GetComponent<VRScrollBar>();
            }
            catch { /* No vertical Scrollbar was found. */ }

            // get VRScrollBarHorizontal if we use the scrollrect as horizontal
            try
            {
                if (horizontal && horizontalScrollbar == null)
                    horizontalScrollbar = transform.FindDeepChild("VRScrollbarHorizontal").GetComponent<VRScrollBar>();
            }
            catch { /* No horizontal Scrollbar was found. */ }

            // get the viewport
            try
            {
                if (viewport == null)
                    viewport = transform.FindDeepChild("Viewport").GetComponent<RectTransform>();
            }
            catch { /* No Viewport was found.*/ }


            // get the content
            try
            {
                if (content == null)
                    content = transform.FindDeepChild("Content").GetComponent<RectTransform>();
            }
            catch { /* No Content was found.*/ }
        }

        private void OnValueChangedCallback()
        {
            _scrollableSetup.CheckContentStatus(viewport, content);
        }
        #endregion
    }
}