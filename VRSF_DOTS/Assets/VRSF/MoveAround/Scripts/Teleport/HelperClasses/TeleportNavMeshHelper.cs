using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Init the values for the Teleport Nav Mesh Classes
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public static class TeleportNavMeshHelper
    {
        #region PUBLIC_METHODS
        /// <summary>
        /// Casts a ray against the Navmesh and attempts to calculate the ray's worldspace intersection with it.
        /// This uses Physics raycasts to perform the raycast calculation, so the teleport surface must have a collider on it.
        /// </summary>
        /// 
        /// <param name="p1">First (origin) point of ray</param>
        /// <param name="p2">Last (end) point of ray</param>
        /// <param name="pointOnNavmesh">If the raycast hit something on the navmesh</param>
        /// <param name="hitPoint">If hit, the point of the hit. Otherwise zero.</param>
        /// <param name="normal">If hit, the normal of the hit surface.  Otherwise (0, 1, 0)</param>
        /// 
        /// <returns>If the raycast hit something.</returns>
        public static bool Linecast(Vector3 p1, Vector3 p2, out bool pointOnNavmesh, int excludedLayer, out Vector3 hitPoint, out Vector3 normal, TeleportNavMesh teleportNavMesh)
        {
            Vector3 dir = p2 - p1;
            float dist = dir.magnitude;
            dir /= dist;

            if (Physics.Raycast(p1, dir, out RaycastHit hit, dist, ~excludedLayer, (QueryTriggerInteraction)teleportNavMesh.QueryTriggerInteraction))
            {
                normal = hit.normal;
                hitPoint = hit.point;

                if (teleportNavMesh.IgnoreSlopedSurfaces && Vector3.Dot(Vector3.up, hit.normal) < 0.99f)
                {
                    pointOnNavmesh = false;
                    return true;
                }
                
                pointOnNavmesh = NavMesh.SamplePosition(hitPoint, out NavMeshHit navHit, teleportNavMesh.SampleRadius, teleportNavMesh.NavAreaMask);
                // Get the closest position on the navMesh
                if (Vector3IsCorrect(navHit.position))
                    hitPoint = navHit.position;
                return true;
            }
            else
            {
                pointOnNavmesh = false;
                hitPoint = Vector3.zero;
                normal = Vector3.up;
                return false;
            }

            bool Vector3IsCorrect(Vector3 posToTest)
            {
                return (posToTest != Vector3.positiveInfinity && posToTest != Vector3.negativeInfinity && float.IsNaN(posToTest.x) && float.IsNaN(posToTest.y) && float.IsNaN(posToTest.z));
            }
        }
        #endregion PUBLIC_METHODS
    }
}