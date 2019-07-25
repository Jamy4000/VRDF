﻿using System.Collections.Generic;
using UnityEngine;
using VRSF.Core.Events;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRInputField element based on the InputField from Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRInputField : TMPro.TMP_InputField
    {
        #region PUBLIC_VARIABLES
        [Header("The VRKeyboard parameters and references")]
        public bool UseVRKeyboard = true;
        public VRKeyboard VRKeyboard;

        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;

        [Tooltip("If this button can be click using a Raycast and the trigger of your controller.")]
        [SerializeField] public bool LaserClickable = true;

        [Tooltip("If this button can be click using the meshcollider of your controller.")]
        [SerializeField] public bool ControllerClickable = true;
        #endregion PUBLIC_VARIABLES

        private TMPro.TMP_Text _placeHolderText;

        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                if (LaserClickable)
                    ObjectWasClickedEvent.Listeners += CheckInputFieldClick;

                if (ControllerClickable)
                    GetComponent<BoxCollider>().isTrigger = true;

                SetInputFieldReferences();

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                    StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckInputFieldClick))
                ObjectWasClickedEvent.Listeners -= CheckInputFieldClick;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ControllerClickable && interactable && other.gameObject.tag.Contains("ControllerBody"))
                StartTyping();
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Method called when the user is clicking
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        void CheckInputFieldClick(ObjectWasClickedEvent clickEvent)
        {
            if (interactable && clickEvent.ObjectClicked == transform)
                StartTyping();
        }

        private void StartTyping()
        {
            _placeHolderText.text = "";
            ActivateInputField();
            CheckForVRKeyboard();
        }

        /// <summary>
        /// Check if a VRKeyboard is used and present in the scene
        /// </summary>
        void CheckForVRKeyboard()
        {
            if (UseVRKeyboard)
            {
                if (VRKeyboard != null)
                {
                    VRKeyboard.InputField = this;
                }
                else
                {
                    try
                    {
                        VRKeyboard = FindObjectOfType<VRKeyboard>();
                        VRKeyboard.InputField = this;
                    }
                    catch
                    {
                        Debug.LogError("The VRKeyboard is not present in the scene." +
                            "Please uncheck the Use VR Keyboard toggle or place a VRKeyboard in the Scene.");
                    }
                }
            }
        }

        /// <summary>
        /// Set the input field reference for the textComponent and the placeHolder
        /// </summary>
        void SetInputFieldReferences()
        {
            try
            {
                if (textComponent == null)
                    textComponent = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
                if (placeholder == null)
                    placeholder = transform.Find("Placeholder").GetComponent<TMPro.TMP_Text>();

                _placeHolderText = (TMPro.TMP_Text)placeholder;
            }
            catch
            {
                Debug.LogError("<b>[VRSF] :</b> Couldn't find the Text and the PlaceHolder in the VRInputField children.");
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

            VRUIBoxColliderSetup.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());
        }
        #endregion PRIVATE_METHODS
    }
}