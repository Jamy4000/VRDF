using UnityEngine;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Stores the Ray and RaycastHit variable for the Left Controller Raycast System
    /// </summary>
    public static class LeftControllerRaycastData
    {
        /// <summary>
        /// Reference to the RaycastHit for when the Left Controller's ray is hitting something
        /// </summary>
        public static RaycastHitVariable RaycastHitVar = new RaycastHitVariable();

        /// <summary>
        /// Reference to the Ray starting from the Left Controller
        /// </summary>
        public static Ray RayVar = new Ray();
    }
}