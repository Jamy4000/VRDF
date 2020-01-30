using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

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
        public static bool Linecast(float3 p1, float3 p2, out bool pointOnNavmesh, out bool pointOnGroundCollider, int excludedLayer, out float3 hitPoint, out float3 normal, TeleportNavMesh tnm)
        {
            Vector3 dir = p2 - p1;
            float dist = dir.magnitude;
            dir /= dist;

            if (Physics.Raycast(p1, dir, out RaycastHit hit, dist, ~excludedLayer, (QueryTriggerInteraction)tnm.QueryTriggerInteraction))
            {
                SampleHitPosition(hit, out pointOnNavmesh, out pointOnGroundCollider, out hitPoint, out normal, tnm);
                return true;
            }
            else if (Physics.SphereCast(p1, tnm.SphereCastRadius, dir, out hit, dist, ~excludedLayer, (QueryTriggerInteraction)tnm.QueryTriggerInteraction))
            {
                SampleHitPosition(hit, out pointOnNavmesh, out pointOnGroundCollider, out hitPoint, out normal, tnm);
                return true;
            }
            else
            {
                pointOnNavmesh = false;
                pointOnGroundCollider = false;
                hitPoint = float3.zero;
                normal = Vector3.up;
                return false;
            }
        }

        private static void SampleHitPosition(RaycastHit hit, out bool pointOnNavmesh, out bool pointOnGroundCollider, out float3 hitPoint, out float3 normal, TeleportNavMesh tnm)
        {
            normal = hit.normal;
            hitPoint = hit.point;

            if (tnm.IgnoreSlopedSurfaces && math.dot(Vector3.up, hit.normal) < 0.99f)
            {
                pointOnNavmesh = false;
                pointOnGroundCollider = false;
            }
            else
            {
                pointOnNavmesh = NavMesh.SamplePosition(hitPoint, out NavMeshHit navHit, tnm.SampleRadius, tnm.NavAreaMask);
                pointOnGroundCollider = hit.transform.gameObject.layer == LayerMask.NameToLayer("Teleportable");

                // Get the closest position on the navMesh
                if (Vector3IsCorrect(navHit.position))
                    hitPoint = navHit.position;
            }
        }

        private static bool Vector3IsCorrect(Vector3 posToTest)
        {
            return FloatIsCorrect(posToTest.x) && FloatIsCorrect(posToTest.y) && FloatIsCorrect(posToTest.z);

            bool FloatIsCorrect(float toTest)
            {
                return !float.IsInfinity(toTest) && !float.IsNaN(toTest);
            }
        }
        #endregion PUBLIC_METHODS
    }
}