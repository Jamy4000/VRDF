using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to have the references to the controllers parameters and capture the inputs
    /// </summary>
    public struct HtcControllersInputCaptureComponent : IComponentData
    {
        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool RightMenuClick;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        public static bool LeftMenuClick;
    }
}