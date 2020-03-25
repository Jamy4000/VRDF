using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VRDF.UI
{
    /// <summary>
    /// Create a new VRDropdown element based on the Dropdown from Unity, but usable in VR with the Scriptable Framework
    /// The Template of the VRDropdown Prefab is modified in a way that the options, when displayed, don't use Toggle but VRToggle
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRDropdown : Dropdown
    {
        #region PUBLIC_VARIABLES
        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        [Tooltip("If this button can be click using the meshcollider of your controller.")]
        [SerializeField] public bool ControllerClickable = true;
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        GameObject _template;
        bool _isShown = false;

        private UnityAction<int> _onValueChangedAction;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
                SetupDropDown();
        }

        protected override void Start()
        {
            base.Start();
            // We setup the BoxCollider size and center
            if (Application.isPlaying && SetColliderAuto)
                StartCoroutine(SetupBoxCollider());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!ControllerClickable || !interactable)
                return;

            if (other.gameObject.CompareTag("ControllerBody") || other.gameObject.CompareTag("UIClicker"))
            {
                SetDropDownNewState();
                UIHapticGenerator.CreateClickHapticSignal(other.name.ToLower().Contains("left") ? Core.Raycast.ERayOrigin.LEFT_HAND : Core.Raycast.ERayOrigin.RIGHT_HAND);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Application.isPlaying)
            {
                if (_onValueChangedAction != null)
                    onValueChanged.RemoveListener(_onValueChangedAction);

                OnVRClickerStartClicking.Listeners -= CheckClickedObject;
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            Core.Raycast.VRRaycastAuthoring.CheckSceneContainsRaycaster();
        }
#endif
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Event called when the DropDown or its children is clicked
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        public void CheckClickedObject(OnVRClickerStartClicking clickEvent)
        {
            // if the clicked object is this one OR if it's something else and this dropdown was shown
            if (interactable && (clickEvent.ClickedObject == gameObject || _isShown))
                SetDropDownNewState();
        }

        /// <summary>
        /// Called when dropdown is click, setup the new state of the element
        /// </summary>
        void SetDropDownNewState()
        {
            if (!_isShown)
                Show();
            else
                Hide();

            _isShown = !_isShown;
        }

        /// <summary>
        /// Setup the BoxCOllider size and center by colling the NotScrollableSetup method CheckBoxColliderSize.
        /// We use a coroutine and wait for the end of the first frame as the element cannot be correctly setup on the first frame
        /// </summary>
        /// <returns></returns>
        IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();
            VRUISetupHelper.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());
        }

        /// <summary>
        /// Set the Dropdown references to the Toggle
        /// </summary>
        private void SetToggleReferences()
        {
            template = _template.GetComponent<RectTransform>();
            captionText = transform.Find("Label").GetComponent<Text>();
            itemText = transform.FindDeepChild("Item Label").GetComponent<Text>();
        }

        /// <summary>
        /// Change the Template to add the VRToggle instead of the one from Unity
        /// </summary>
        private void ChangeTemplate()
        {
            _template.SetActive(true);

            Transform item = _template.transform.Find("Viewport/Content/Item");

            if (item.GetComponent<VRToggle>() == null)
            {
                DestroyImmediate(item.GetComponent<Toggle>());

                VRToggle newToggle = item.gameObject.AddComponent<VRToggle>();
                newToggle.targetGraphic = item.Find("Item Background").GetComponent<Image>();
                newToggle.isOn = true;
                newToggle.toggleTransition = Toggle.ToggleTransition.Fade;
                newToggle.graphic = item.Find("Item Checkmark").GetComponent<Image>();
            }

            _template.SetActive(false);
        }

        private void SetupDropDown()
        {
            _onValueChangedAction = delegate { SetDropDownNewState(); };
            onValueChanged.AddListener(_onValueChangedAction);

            OnSetupVRReady.RegisterSetupVRCallback(CheckDevice);

            // We setup the Template and Options to fit the VRFramework
            _template = transform.Find("Template").gameObject;
            SetToggleReferences();
            ChangeTemplate();
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            if (OnSetupVRReady.IsCallbackRegistered(CheckDevice))
                OnSetupVRReady.Listeners -= CheckDevice;

            if (VRDF_Components.DeviceLoaded != Core.SetupVR.EDevice.SIMULATOR || VRUISetupHelper.ShouldRegisterForSimulator(this))
                OnVRClickerStartClicking.Listeners += CheckClickedObject;
        }
        #endregion PRIVATE_METHODS
    }
}