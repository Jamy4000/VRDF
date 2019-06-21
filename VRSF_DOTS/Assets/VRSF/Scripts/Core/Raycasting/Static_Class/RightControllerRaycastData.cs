using UnityEngine;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Stores the Ray and RaycastHit variable for the Right Controller Raycast System
    /// </summary>
    public static class RightControllerRaycastData
    {
        /// <summary>
        /// Reference to the RaycastHit for when the Right Controller's ray is hitting something
        /// </summary>
        public static RaycastHitVariable RaycastHitVar = new RaycastHitVariable();

        /// <summary>
        /// Reference to the Ray starting from the Right Controller
        /// </summary>
        public static Ray RayVar = new Ray();
    }
}