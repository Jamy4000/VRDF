using System.Collections.Generic;
using UnityEngine;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRInputField element based on the InputField from Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRInputField : TMPro.TMP_InputField
    {
        #region VARIABLES
        [Header("The VRKeyboard parameters and references")]
        public bool UseVRKeyboard = true;
        public VRKeyboard VRKeyboard;

        [Tooltip("If you want to set the collider's size yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;

        [Tooltip("If this button can be click using a Raycast and the trigger of your controller.")]
        [SerializeField] public bool LaserClickable = true;

        [Tooltip("If this button can be click using the meshcollider of your controller.")]
        [SerializeField] public bool ControllerClickable = true;
        #endregion VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                if (LaserClickable && VRUISetupHelper.ShouldRegisterForSimulator(this))
                    OnVRClickerStartClicking.Listeners += CheckClickedObject;

                if (ControllerClickable)
                    GetComponent<BoxCollider>().isTrigger = true;

                SetInputFieldReferences();
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
            if (OnVRClickerStartClicking.IsCallbackRegistered(CheckClickedObject))
                OnVRClickerStartClicking.Listeners -= CheckClickedObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!ControllerClickable || !interactable)
                return;

            if (other.gameObject.CompareTag("ControllerBody") || other.gameObject.CompareTag("UIClicker"))
            {
                StartTyping();
                UIHapticGenerator.CreateClickHapticSignal(other.name.ToLower().Contains("left") ? Core.Raycast.ERayOrigin.LEFT_HAND : Core.Raycast.ERayOrigin.RIGHT_HAND);
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
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="startClickingEvent">The event raised when an object was clicked</param>
        private void CheckClickedObject(OnVRClickerStartClicking startClickingEvent)
        {
            if (interactable && startClickingEvent.ClickedObject == gameObject)
            {
                StartTyping();
                UIHapticGenerator.CreateClickHapticSignal(startClickingEvent.RaycastOrigin);
            }
            else
            {
                m_CaretVisible = false;
            }
        }

        public void StartTyping()
        {
            ActivateInputField();
            CheckForVRKeyboard();
            m_CaretVisible = true;
        }

        /// <summary>
        /// Check if a VRKeyboard is used and present in the scene
        /// </summary>
        private void CheckForVRKeyboard()
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
        private void SetInputFieldReferences()
        {
            try
            {
                if (textComponent == null)
                    textComponent = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
                if (placeholder == null)
                    placeholder = transform.Find("Placeholder").GetComponent<TMPro.TMP_Text>();
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
            VRUISetupHelper.CheckBoxColliderSize(GetComponent<BoxCollider>(), GetComponent<RectTransform>());
        }
        #endregion PRIVATE_METHODS
    }
}