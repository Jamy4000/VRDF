using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// 
    /// </summary>
    public struct DeviceToLoad : IComponentData
    {
        [Header("VR Device Parameters.")]
        [Tooltip("The Device you want to load.")]
        [SerializeField]
        public EDevice Device;

        [Tooltip("If false, the device to load will be set with your Editor choice or with a potential starting screen choice.")]
        [SerializeField]
        public bool ShouldCheckConnectedDevice;

        /// <summary>
        /// Check if we already instantiated the SDK in the past, useful if the SDK is re-instantiated after a new scene has been loaded
        /// </summary>
        [HideInInspector] public static bool IsLoaded;
    }
}