using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A helper class containing the mathematics to calculate a parabole curve.
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public static class ParaboleCalculationsHelper
    {
        /// <summary>
        ///  Used when you can't depend on Update() to automatically update CurrentParabolaAngle
        /// (for example, directly after enabling the component)
        /// </summary>
        public static float3 ForceUpdateCurrentAngle(CurveTeleporterCalculations pointerCalculations, Vector3 velocity)
        {
            //Vector3 velocity = e.PointerObjects.transform.TransformDirection(PointerCalculations.InitialVelocity);
            pointerCalculations.CurrentParabolaAngleY = ClampInitialVelocity(ref velocity, out float3 d);//, PointerCalculations.InitialVelocity);
            pointerCalculations.CurrentPointVector = d;
            return velocity;
        }


        /// <summary>
        /// Clamps the given velocity vector so that it can't be more than 45 degrees above the horizontal.
        /// This is done so that it is easier to leverage the maximum distance (at the 45 degree angle) of
        /// parabolic motion.
        /// </summary>
        /// <returns>angle with reference to the XZ plane</returns>
        private static float ClampInitialVelocity(ref Vector3 velocity, out float3 velocity_normalized)//, float3 initialVelocity)
        {
            // Project the initial velocity onto the XZ plane.  This gives us the "forward" direction
            Vector3 velocity_fwd = ProjectVectorOntoPlane(Vector3.up, velocity);

            // Find the angle between the XZ plane and the velocity
            float angle = Vector3.Angle(velocity_fwd, velocity);
            // Calculate positivity/negativity of the angle using the cross product
            // Below is "right" from controller's perspective (could also be left, but it doesn't matter for our purposes)
            Vector3 right = Vector3.Cross(Vector3.up, velocity_fwd);
            // If the cross product between forward and the velocity is in the same direction as right, then we are below the vertical
            if (Vector3.Dot(right, Vector3.Cross(velocity_fwd, velocity)) > 0)
                angle *= -1;

            // Clamp the angle if it is greater than 45 degrees
            //if (angle > 45)
            //{
            //    velocity = Vector3.Slerp(velocity_fwd, velocity, 45f / angle);
            //    velocity /= velocity.magnitude;
            //    velocity_normalized = velocity;
            //    velocity *= initialVelocity.magnitude;
            //    angle = 45;
            //}
            //else
            velocity_normalized = velocity.normalized;

            return angle;
        }

        /// <summary>
        /// Project a point on a normalized plane
        /// </summary>
        /// <param name="planeNormal">The normalized Vector</param>
        /// <param name="point">The point to project</param>
        /// <returns>The new projected Vector</returns>
        private static Vector3 ProjectVectorOntoPlane(Vector3 planeNormal, Vector3 point)
        {
            Vector3 d = Vector3.Project(point, planeNormal.normalized);
            return point - d;
        }

        /// <summary>
        /// Calculate the points on the way of the Parabola
        /// </summary>
        /// <param name="e">The reference to the entity to check</param>
        /// <param name="velocity">The velocity of the Parabole</param>
        /// <returns>The normal of the Curve</returns>
        public static float3 ParabolaPointsCalculations(ref NativeArray<Translation> pp, ref CurveTeleporterCalculations ctc, ParabolPointsParameters ppp, TeleportNavMesh tnm, float3 parabolOrigin, LayerMask excludedLayer, Vector3 velocity)
        {
            ctc.PointOnNavMesh = CalculateParabolicCurve
            (
                parabolOrigin,
                velocity,
                ctc.Acceleration,
                ppp.PointSpacing,
                ppp.PointCount,
                tnm,
                excludedLayer,
                out pp,
                out float3 normal,
                out ctc.LastPointIndex,
                out bool onTeleportableLayer
            );

            ctc.PointOnTeleportableLayer = onTeleportableLayer;
            ctc.PointToGoTo = pp[ctc.LastPointIndex].Value;
            return normal;
        }

        /// <summary>
        /// Sample a bunch of points along a parabolic curve until you hit gnd.  At that point, cut off the parabola
        /// </summary>
        /// 
        /// <param name="p0">starting point of parabola</param>
        /// <param name="v0">initial parabola velocity</param>
        /// <param name="a">initial acceleration</param>
        /// <param name="dist">distance between sample points</param>
        /// <param name="points">number of sample points</param>
        /// <param name="tnm">Vive Nav Mesh used to teleport</param>
        /// <param name="outPts">List that will be populated by new points</param>
        /// <param name="normal">normal of hit point</param>
        /// 
        /// <returns>true if the the parabole is at the end of the NavMesh</returns>
        private static bool CalculateParabolicCurve(float3 p0, Vector3 v0, Vector3 a, float dist, int points, TeleportNavMesh tnm, int excludedLayer, out NativeArray<Translation> outPts, out float3 normal, out int lastPointIndex, out bool endOnTeleportableLayer)
        {
            // Init new list of points with p0 as the first point
            outPts = new NativeArray<Translation>(points, Allocator.TempJob);
            outPts[0] = new Translation { Value = p0 };

            float3 last = p0;
            float t = 0;

            for (int i = 0; i < points; i++)
            {
                t += dist / ParabolicCurveDeriv(v0, a, t).magnitude;
                float3 next = ParabolicCurve(p0, v0, a, t);

                if (TeleportNavMeshHelper.Linecast(last, next, out bool endOnNavmesh, out endOnTeleportableLayer, excludedLayer, out float3 castHit, out float3 norm, tnm))
                {
                    outPts[i] = new Translation { Value = castHit };
                    normal = norm;
                    lastPointIndex = i;
                    return endOnNavmesh;
                }
                else
                {
                    outPts[i] = new Translation { Value = next };
                }

                last = next;
            }

            normal = Vector3.up;
            lastPointIndex = points - 1;
            endOnTeleportableLayer = false;
            return false;
        }

        /// <summary>
        /// Parabolic motion equation, y = p0 + v0*t + 1/2at^2
        /// </summary> 
        private static float ParabolicCurve(float p0, float v0, float a, float t)
        {
            return p0 + v0 * t + 0.5f * a * t * t;
        }

        /// <summary>
        /// Derivative of parabolic motion equation
        /// </summary> 
        private static float ParabolicCurveDeriv(float v0, float a, float t)
        {
            return v0 + a * t;
        }

        /// <summary>
        /// Parabolic motion equation applied to 3 dimensions
        /// </summary> 
        private static Vector3 ParabolicCurve(Vector3 p0, Vector3 v0, Vector3 a, float t)
        {
            Vector3 ret = new Vector3();
            for (int x = 0; x < 3; x++)
                ret[x] = ParabolicCurve(p0[x], v0[x], a[x], t);
            return ret;
        }

        /// <summary>
        /// Parabolic motion derivative applied to 3 dimensions
        /// </summary> 
        private static Vector3 ParabolicCurveDeriv(Vector3 v0, Vector3 a, float t)
        {
            Vector3 ret = new Vector3();
            for (int x = 0; x < 3; x++)
                ret[x] = ParabolicCurveDeriv(v0[x], a[x], t);
            return ret;
        }
    }
}