using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.VRInteractions;
using VRSF.Core.Raycast;

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
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                _scrollableSetup = new VRUIScrollableSetup(Direction);
                SetupVRScrollRect();
                _scrollableSetup.CheckContentStatus(viewport, content);
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
            if (verticalScrollbar != null)
                verticalScrollbar.onValueChanged.RemoveAllListeners();

            if (horizontalScrollbar != null)
                horizontalScrollbar.onValueChanged.RemoveAllListeners();

            OnVRClickerStartClicking.Listeners -= CheckClickedObject;
            OnVRClickerStopClicking.Listeners -= CheckUnclickedObject;
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (_rayHoldingHandle != ERayOrigin.NONE)
                {
                    if (vertical && verticalScrollbar)
                        verticalScrollbar.value = CheckScrollbarValue();
                    else if (horizontal && horizontalScrollbar)
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
                    throw new System.Exception();
            }
        }

        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="startClickingEvent">The event raised when an object was clicked</param>
        private void CheckClickedObject(OnVRClickerStartClicking startClickingEvent)
        {
            if (CanHoldHandle())
            {
                _rayHoldingHandle = startClickingEvent.RaycastOrigin;
                UIHapticGenerator.CreateClickHapticSignal(_rayHoldingHandle);
            }

            bool CanHoldHandle()
            {
                return startClickingEvent.ClickedObject == gameObject && _rayHoldingHandle == ERayOrigin.NONE;
            }
        }

        /// <summary>
        /// Check whenever an object is unclicked if it correspond to this gameObject.
        /// </summary>
        /// <param name="stopClickingEvent"></param>
        private void CheckUnclickedObject(OnVRClickerStopClicking stopClickingEvent)
        {
            if (stopClickingEvent.UnclickedObject == gameObject)
                _rayHoldingHandle = ERayOrigin.NONE;
        }

        /// <summary>
        /// Start a coroutine that wait for the second frame to set the BoxCollider
        /// </summary>
        /// <returns></returns>
        IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
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
        }

        private void SetupVRScrollRect()
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

            OnVRClickerStartClicking.Listeners += CheckClickedObject;
            OnVRClickerStopClicking.Listeners += CheckUnclickedObject;

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