using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Check the Raycast of the two controllers and the Camera/Gaze, and reference them in RaycastHit and Ray static classes
    /// Cannot be jobify as we need to be in the MainThread to get the physic from Unity (the Hit and colliders)
    /// </summary>
    public class RaycastHitCalculationsSystem : ComponentSystem
    {
        /// <summary>
        /// Check every frame if the Ray from a controller/the camera is hitting something
        /// </summary>
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, ref VRRaycastParameters parameters, ref VRRaycastOutputs raycastOutputs, ref VRRaycastOrigin origin) =>
            {
                var rayHitSomething = Physics.Raycast(raycastOutputs.RayVar, out raycastOutputs.RaycastHitVar.Value, parameters.MaxRaycastDistance, ~parameters.ExcludedLayer);
                raycastOutputs.RaycastHitVar.SetIsNull(!rayHitSomething);

                //var hits = Physics.RaycastAll(raycastOutputs.RayVar, parameters.MaxRaycastDistance, ~parameters.ExcludedLayer);

                //if (hits.Length > 0)
                //{
                //    var first3DHit = hits.OrderBy(x => x.distance).First();
                //    raycastOutputs.RaycastHitVar.Value = first3DHit;
                //    raycastOutputs.RaycastHitVar.SetIsNull(false);
                //}
                //else
                //{
                //    raycastOutputs.RaycastHitVar.SetIsNull(true);
                //}
            });
        }
    }
}