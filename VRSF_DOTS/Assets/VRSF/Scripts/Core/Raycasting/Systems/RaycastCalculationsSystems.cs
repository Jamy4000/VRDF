using ScriptableFramework.Variables;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Check the Raycast of the two controllers and the Gaze and reference them in RaycastHit and Ray ScriptableVariable
    /// </summary>
    public class RaycastCalculationsSystems : ComponentSystem
    {
        struct Filter
        {
            public ScriptableRaycastComponent RaycastComponents;
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.RaycastComponents.IsSetup)
                {
                    Ray ray = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR ? e.RaycastComponents._VRCamera.ScreenPointToRay(Input.mousePosition) : new Ray(e.RaycastComponents.RayOriginTransform.position, e.RaycastComponents.RayOriginTransform.TransformDirection(Vector3.forward));

                    e.RaycastComponents.RayVar.SetValue(ray);

                    RaycastHitHandler(ray, e.RaycastComponents.MaxRaycastDistance, e.RaycastComponents.RayOrigin, ~e.RaycastComponents.ExcludedLayer, ref e.RaycastComponents.RaycastHitVar);
                }
            }
        }


        #region PRIVATE_METHODS  
        /// <summary>
        /// Check if the Ray from a controller is hitting something
        /// </summary>
        /// <param name="ray">The ray to check</param>
        /// <param name="distance">The maximum distance to which we raycast</param>
        /// <param name="layerToIgnore">The layer(s) to ignore from raycasting</param>
        /// <param name="hitVariable">The RaycastHitVariable in which we store the hit value</param>
        private void RaycastHitHandler(Ray ray, float distance, ERayOrigin rayOrigin, int layerToIgnore, ref RaycastHitVariable hitVariable)
        {
            var hits = Physics.RaycastAll(ray, distance, layerToIgnore);

            if (hits.Length > 0)
            {
                var first3DHit = hits.OrderBy(x => x.distance).First();
                hitVariable.SetValue(first3DHit);
                hitVariable.SetIsNull(false);
            }
            else
            {
                hitVariable.SetIsNull(true);
            }
        }
        #endregion PRIVATE_METHODS
    }
}