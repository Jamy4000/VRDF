using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to have the references to the controllers parameters and capture the inputs for the Rift, Rift S and Quest
    /// </summary>
    public struct OculusControllersInputCaptureComponent : Unity.Entities.IComponentData
    {
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool LeftMenuClick;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool AButtonClick;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool AButtonTouch;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool BButtonClick;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool BButtonTouch;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool XButtonClick;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool XButtonTouch;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool YButtonClick;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool YButtonTouch;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool RightThumbrestTouch;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool LeftThumbrestTouch;
    }
}