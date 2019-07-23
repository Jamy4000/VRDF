using VRSF.Core.Utils;
using VRSF.Core.Controllers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using VRSF.Core.Events;
using UnityEngine.Events;

namespace VRSF.UI
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
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        GameObject _template;
        bool _isShown = false;

        /// <summary>
        /// true when the events for ObjectWasClicked or Hovered were registered.
        /// </summary>
        private bool _eventWereRegistered;

        private UnityAction<int> _onValueChangedAction;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                SetupUIElement();

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                    StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_eventWereRegistered)
            {
                onValueChanged.RemoveListener(_onValueChangedAction);
                ObjectWasClickedEvent.UnregisterListener(CheckObjectClicked);
                _eventWereRegistered = false;
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        private void SetupUIElement()
        {
            _onValueChangedAction = delegate { SetDropDownNewState(); };
            onValueChanged.AddListener(_onValueChangedAction);
            ObjectWasClickedEvent.RegisterListener(CheckObjectClicked);

            _eventWereRegistered = true;

            // We setup the Template and Options to fit the VRFramework
            _template = transform.Find("Template").gameObject;
            SetToggleReferences();
            ChangeTemplate();
        }

        /// <summary>
        /// Event called when the DropDown or its children is clicked
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        public void CheckObjectClicked(ObjectWasClickedEvent clickEvent)
        {
            if (interactable && clickEvent.ObjectClicked == transform)
            {
                SetDropDownNewState();
            }
        }

        /// <summary>
        /// Called when dropdown is click, setup the new state of the element
        /// </summary>
        void SetDropDownNewState()
        {
            if (!_isShown)
            {
                Show();
                _isShown = true;
            }
            else
            {
                Hide();
                _isShown = false;
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

            BoxCollider box = GetComponent<BoxCollider>();
            box = VRUIBoxColliderSetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
        }

        /// <summary>
        /// Set the Dropdown references to the Toggle
        /// </summary>
        void SetToggleReferences()
        {
            template = _template.GetComponent<RectTransform>();
            captionText = transform.Find("Label").GetComponent<Text>();
            itemText = transform.FindDeepChild("Item Label").GetComponent<Text>();
        }

        /// <summary>
        /// Change the Template to add the VRToggle instead of the one from Unity
        /// </summary>
        void ChangeTemplate()
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
        #endregion PRIVATE_METHODS
    }
}