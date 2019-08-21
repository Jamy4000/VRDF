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
        [Header("Event fired when the user is clicking the Enter button")]
        public UnityEvent OnEnterClicked;

        protected virtual void Awake()
        {
            foreach (VRButton button in GetComponentsInChildren<VRButton>())
            {
                button.onClick.AddListenerExtend(delegate { ClickKey(button.name); });
            }
        }

        /// <summary>
        /// Called when a key on the keyboard is clicked
        /// </summary>
        /// <param name="character">The character that was clicked</param>
        protected virtual void ClickKey(string character)
        {
            if (!InputField) return;

            switch (character.ToLower())
            {
                case "backspace":
                    Backspace();
                    break;
                case "enter":
                    Enter();
                    break;
                case "space":
                    InputField.text += " ";
                    break;
                default:
                    InputField.text += character;
                    break;
            }
        }

        /// <summary>
        /// Handle the Backspace Key
        /// </summary>
        protected virtual void Backspace()
        {
            if (InputField.text.Length > 0)
                InputField.text = InputField.text.Substring(0, InputField.text.Length - 1);
        }

        /// <summary>
        /// Handle the Enter Key
        /// </summary>
        protected virtual void Enter()
        {
            OnEnterClicked.Invoke();
        }


        public TMPro.TMP_InputField InputField { get; set; }
    }
}