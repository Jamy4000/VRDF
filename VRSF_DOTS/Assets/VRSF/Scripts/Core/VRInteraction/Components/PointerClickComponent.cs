using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Interactions
{
    /// <summary>
    /// Contains the variables for the OnColliderClickSystems
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class PointerClickComponent : MonoBehaviour
    {
        public EHand HandClicking = EHand.NONE;

        /// <summary>
        /// Whether the user is able to click on stuffs with the left trigger
        /// </summary>
        public static bool LeftTriggerCanClick = true;

        /// <summary>
        /// Whether the user is able to click on stuffs with the right trigger
        /// </summary>
        public static bool RightTriggerCanClick = true;
    }
}