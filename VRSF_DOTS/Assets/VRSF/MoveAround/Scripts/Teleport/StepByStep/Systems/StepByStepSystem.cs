using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Using the ButtonActionChoser, this System allow the user to move Step by Step, ie in the direction of its laser to which this feature is linked.
    /// </summary>
    public class StepByStepSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (!VRSF_Components.SetupVRIsReady)
                return;

            float3 newPos = float3.zero;
            bool canTeleport = false;

            Entities.ForEach((ref StepByStepComponent sbs, ref VRRaycastParameters raycastParam, ref GeneralTeleportParameters gtp, ref TeleportNavMesh tnm, ref VRRaycastOutputs raycastOutputs) =>
            {
                switch (gtp.CurrentTeleportState)
                {
                    case ETeleportState.Teleporting:
                        canTeleport = UserIsOnNavMesh(sbs, tnm, raycastOutputs, raycastParam.ExcludedLayer, out Vector3 newUsersPos);
                        newPos = newUsersPos;

                        // We teleport the user as soon as we go out of the job
                        gtp.HasTeleported = true;
                        break;
                    default:
                        canTeleport = false;
                        break;
                }
            });

            if (canTeleport) VRSF_Components.SetCameraRigPosition(newPos, false);
        }

        private bool UserIsOnNavMesh(StepByStepComponent sbs, TeleportNavMesh tnm, VRRaycastOutputs raycastOutputs, LayerMask excludedLayers, out Vector3 newCameraRigPos)
        {
            Transform vrCamTransform = VRSF_Components.VRCamera.transform;
            Transform cameraRigTransform = VRSF_Components.CameraRig.transform;

            // We calculate the direction and the distance Vectors
            var directionVector = raycastOutputs.RayVar.direction;
            float distanceVector = cameraRigTransform.localScale.y * sbs.DistanceStepByStep;

            // Check if we hit a collider on the way. If it's the case, we reduce the distance.
            if (Physics.Raycast(vrCamTransform.position, directionVector, out RaycastHit hit, distanceVector, ~excludedLayers))
                distanceVector = hit.distance - 0.1f;

            // We multiply the direction vector by the distance to which the user should be going
            directionVector *= distanceVector;

            // We check the theoritic position for the cameraRig
            newCameraRigPos = GetNewTheoriticPos(directionVector, true);
            // We check the theoritic new user pos
            var newCameraPos = GetNewTheoriticPos(directionVector, false);

            // We calculate a vector down based on the new Camera Pos. 
            var downVectorDistance = Mathf.Abs(vrCamTransform.localPosition.y + VRSF_Components.FloorOffset.transform.localPosition.y) + sbs.StepHeight;
            var downVector = newCameraPos + (Vector3.down * downVectorDistance);

            // We calculate the linecast between the newUserPos and the downVector and check if it hits the NavMesh
            TeleportNavMeshHelper.Linecast
            (
                newCameraPos,
                downVector,
                out bool endOnNavmesh,
                excludedLayers,
                out newCameraPos,
                out _,
                tnm
            );

            // We set the camera rig pos in y to the camera pos in y
            newCameraRigPos.y = newCameraPos.y;

            return endOnNavmesh;


            /// <summary>
            /// We get the theoritic position for the CameraRig and the VRCamera based on the scaled direction (direction * distance)
            /// </summary>
            /// <param name="scaledDirection">The direction multiplied by the distance to go to</param>
            /// <param name="isCheckingCameraRig">Whether the check is for the CameraRig or the VRCamera</param>
            /// <returns>The new Theoritic position</returns>
            Vector3 GetNewTheoriticPos(Vector3 scaledDirection, bool isCheckingCameraRig)
            {
                var origin = isCheckingCameraRig ? cameraRigTransform.position : vrCamTransform.position;
                return origin + new Vector3(scaledDirection.x, 0.0f, scaledDirection.z);
            }
        }
    }
}