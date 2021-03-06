﻿using UnityEngine;

namespace VRDF.UI
{
    /// <summary>
    /// Automatically select an InputField when it's enabled in scene
    /// </summary>
    [RequireComponent(typeof(VRInputField))]
    public class InputFieldSelector : MonoBehaviour
    {
        /// <summary>
        /// The input field attached to this script
        /// </summary>
        private VRInputField _inputField;

        private void Awake()
        {
            _inputField = GetComponent<VRInputField>();
        }

        private void OnEnable()
        {
            _inputField.StartTyping();
        }

        private void OnDisable()
        {
            _inputField.DeactivateInputField();
        }
    }
}