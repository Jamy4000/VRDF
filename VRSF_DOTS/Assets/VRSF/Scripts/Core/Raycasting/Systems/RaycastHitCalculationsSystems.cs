using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Check the Raycast of the two controllers and the Camera/Gaze, and reference them in RaycastHit and Ray static classes
    /// Cannot be jobify as we need to be in the MainThread to get the physic from Unity (the Hit and colliders)
    /// </summary>
    public class RaycastHitCalculationsSystems : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref VRRaycastOrigin raycastOrigin, ref VRRaycastParameters parameters) =>
            {
                // Depending on the RayOring, we provide references to different ray and raycastHit variables
                switch (raycastOrigin.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        RaycastHitHandler(parameters, LeftControllerRaycastData.RayVar, ref LeftControllerRaycastData.RaycastHitVar);
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        RaycastHitHandler(parameters, RightControllerRaycastData.RayVar, ref RightControllerRaycastData.RaycastHitVar);
                        break;
                    case ERayOrigin.CAMERA:
                        RaycastHitHandler(parameters, CameraRaycastData.RayVar, ref CameraRaycastData.RaycastHitVar);
                        break;

                    default:
                        Debug.LogError("[b]VRSF :[\b] An error has occured in the RaycastHitCalculationsSystems. " +
                            "Please check that the RayOrigin for your VRRaycatAuthoring Components are set correctly.");
                        break;
                }
            });
        }


        /// <summary>
        /// Check if the Ray from a controller/the camera is hitting something
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="ray">The ray to check</param>
        /// <param name="hitVariable">The RaycastHitVariable in which we store the hit value</param>
        private void RaycastHitHandler(VRRaycastParameters parameters, Ray ray, ref RaycastHitVariable hitVariable)
        {
            var hits = Physics.RaycastAll(ray, parameters.MaxRaycastDistance, ~parameters.ExcludedLayer);

            if (hits.Length > 0)
            {
                var first3DHit = hits.OrderBy(x => x.distance).First();
                hitVariable.Value = first3DHit;
                hitVariable.SetIsNull(false);
            }
            else
            {
                hitVariable.SetIsNull(true);
            }
        }
    }
}