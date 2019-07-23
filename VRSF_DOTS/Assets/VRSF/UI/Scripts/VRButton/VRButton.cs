using UnityEngine;
using System.Collections.Generic;
using VRSF.Core.Events;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRButton based on the Button for Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRButton : UnityEngine.UI.Button
    {
        #region PUBLIC_VARIABLES
        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;

        [Tooltip("If this button can be click using a Raycast and the trigger of your controller.")]
        [SerializeField] public bool LaserClickable = true;

        [Tooltip("If this button can be click using the meshcollider of your controller.")]
        [SerializeField] public bool ControllerClickable = true;
        #endregion PUBLIC_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                if (LaserClickable)
                    ObjectWasClickedEvent.Listeners += CheckObjectClicked;

                if (ControllerClickable)
                    GetComponent<BoxCollider>().isTrigger = true;

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                    StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (ObjectWasClickedEvent.IsMethodAlreadyRegistered(CheckObjectClicked))
                ObjectWasClickedEvent.Listeners -= CheckObjectClicked;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ControllerClickable && interactable && other.gameObject.tag.Contains("ControllerBody"))
                onClick.Invoke();
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