using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to have the references to the controllers parameters and capture the inputs for the Rift, Rift S and Quest
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class OculusControllersInputCaptureComponent : MonoBehaviour
    {
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable LeftMenuClick;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable AButtonClick;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable AButtonTouch;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable BButtonClick;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable BButtonTouch;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable XButtonClick;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable XButtonTouch;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable YButtonClick;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable YButtonTouch;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable RightThumbrestTouch;
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable LeftThumbrestTouch;
    }
}