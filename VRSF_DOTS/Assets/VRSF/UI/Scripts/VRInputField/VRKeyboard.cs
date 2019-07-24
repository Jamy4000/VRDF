using UnityEngine;
using UnityEngine.Events;
using VRSF.Core.Utils;

namespace VRSF.UI
{
    /// <summary>
    /// Script placed on the VRKeyboard Prefab. 
    /// It handle the different keys clicked on the VRKeyboard when an InputField is selected.
    /// </summary>
    public class VRKeyboard : MonoBehaviour
    {
        #region PUBLIC_VARIABLES 
        [Header("Event fired when the user is clicking the Enter button")]
        public UnityEvent OnEnterClicked;
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        [Tooltip("This can be set via a script by referencing the VRKeyboard")]
        private TMPro.TMP_InputField inputField;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        private void Awake()
        {
            VRButton[] buttons = GetComponentsInChildren<VRButton>();
            foreach (VRButton button in buttons)
            {
                button.onClick.AddListenerExtend(delegate { ClickKey(button.name); });
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Called when a key on the keyboard is clicked
        /// </summary>
        /// <param name="character">The character that was clicked</param>
        private void ClickKey(string character)
        {
            if (!inputField) return;

            switch (character)
            {
                case "Backspace":
                    Backspace();
                    break;
                case "Enter":
                    Enter();
                    break;
                case "Space":
                    inputField.text += " ";
                    break;
                default:
                    inputField.text += character;
                    break;
            }
        }

        /// <summary>
        /// Handle the Backspace Key
        /// </summary>
        private void Backspace()
        {
            if (inputField.text.Length > 0)
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            }
        }

        /// <summary>
        /// Handle the Enter Key
        /// </summary>
        private void Enter()
        {
            OnEnterClicked.Invoke();
            Debug.Log("You've typed [" + inputField.text + "]");
        }
        #endregion PRIVATE_METHODS


        #region GETTERS_SETTERS
        public TMPro.TMP_InputField InputField
        {
            get
            {
                return inputField;
            }

            set
            {
                inputField = value;
            }
        }
        #endregion GETTERS_SETTERS
    }
}