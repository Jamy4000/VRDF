using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to have the references to the controllers parameters and capture the inputs for the GearVR and the Oculus Go
    /// </summary>
    public struct GoAndGearVRControllersInputCaptureComponent : IComponentData
    {
        [Tooltip("Whether you want the controller to be shown on the right or left hand. WARNING : If you want to change the dominant hand, raise the ChangeDominantHandEvent. Everything is then automatically handled in the DominantHandHandlerSystem.")]
        public bool IsUserRightHanded;
        
        public static bool BackButtonClick;
    }
}