using UnityEngine;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Stores the Ray and RaycastHit variable for the Camera Raycast System, or Gaze
    /// </summary>
    public static class CameraRaycastData
    {
        /// <summary>
        /// Reference to the RaycastHit for when the Camera's ray is hitting something
        /// </summary>
        public static RaycastHitVariable RaycastHitVar = new RaycastHitVariable();

        /// <summary>
        /// Reference to the Ray starting from the Camera
        /// </summary>
        public static Ray RayVar = new Ray();
    }
}