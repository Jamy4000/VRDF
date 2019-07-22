using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.FadingEffect;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// This System allow the user to move Step by Step, ie in the direction of its laser to which this feature is linked.
    /// </summary>
    public class StepByStepSystem : ComponentSystem
    {
        private float3 _tempPointToGoTo;

        protected override void OnUpdate()
        {
            Entities.ForEach((ref StepByStepComponent sbs, ref VRRaycastParameters raycastParam, ref GeneralTeleportParameters gtp, ref TeleportNavMesh tnm, ref VRRaycastOutputs raycastOutputs) =>
            {
                if (gtp.CurrentTeleportState == ETeleportState.Teleporting)
                {
                    // We teleport the user as soon as we go out of the job
                    gtp.HasTeleported = true;
                    if (UserIsOnNavMesh(sbs, tnm, raycastOutputs, raycastParam.ExcludedLayer, out float3 newUsersPos))
                    {
                        if (gtp.IsUsingFadingEffect)
                        {
                            OnFadingOutEndedEvent.Listeners += TeleportUser;
                            _tempPointToGoTo = newUsersPos;
                            new StartFadingOutEvent(true);
                        }
                        else
                        {
                            VRSF_Components.SetCameraRigPosition(newUsersPos);
                        }
                    }
                }
            });
        }

        private bool UserIsOnNavMesh(StepByStepComponent sbs, TeleportNavMesh tnm, VRRaycastOutputs raycastOutputs, LayerMask excludedLayers, out float3 newCameraRigPos)
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
            float3 newCameraPos = GetNewTheoriticPos(directionVector, false);

            // We calculate a vector down based on the new Camera Pos. 
            var downVectorDistance = Mathf.Abs(vrCamTransform.localPosition.y + VRSF_Components.FloorOffset.transform.localPosition.y) + sbs.StepHeight;
            var downVector = newCameraPos + (new float3(0.0f, -1.0f, 0.0f) * downVectorDistance);

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
            float3 GetNewTheoriticPos(Vector3 scaledDirection, bool isCheckingCameraRig)
            {
                float3 origin = isCheckingCameraRig ? cameraRigTransform.position : vrCamTransform.position;
                return origin + new float3(scaledDirection.x, 0.0f, scaledDirection.z);
            }
        }

        private void TeleportUser(OnFadingOutEndedEvent info)
        {
            OnFadingOutEndedEvent.Listeners -= TeleportUser;
            VRSF_Components.SetCameraRigPosition(_tempPointToGoTo);
        }
    }
}