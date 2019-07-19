using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class ParabolRenderingSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // TODO
            return inputDeps;
        }

        struct ParaboleCalculationsJob : IJobForEach<CurveTeleporterCalculations, ParabolPointParameter, ParabolCalculations, GeneralTeleportParameters, TeleportNavMesh, VRRaycastParameters>
        {
            // TODO : float3 BaseVelocity = e.PointerObjects.transform.TransformDirection(PointerCalculations.InitialVelocity);
            public float3 BaseVelocity;
            public float3 ParabolOrigin;

            public void Execute(ref CurveTeleporterCalculations ctc, ref ParabolPointParameter ppp, ref ParabolCalculations parabolCalc, ref GeneralTeleportParameters gtp, ref TeleportNavMesh tnm, ref VRRaycastParameters raycastParam)
            {
                if (gtp.CurrentTeleportState == ETeleportState.Selecting)
                {
                    //// 2. Render the Parabole's pads, aka the targets at the end of the parabole
                    //ParabolicRendererHelper.RenderParabolePads(e, normal);

                    //// 3. Draw parabola (BEFORE the outside faces of the selection pad, to avoid depth issues)
                    //ParaboleCalculationsHelper.GenerateMesh(ref e.PointerObjects._parabolaMesh, e.PointerObjects.ParabolaPoints, velocity, Time.time % 1, e.PointerCalculations.GraphicThickness);
                    //Graphics.DrawMesh(e.PointerObjects._parabolaMesh, Matrix4x4.identity, e.PointerCalculations.GraphicMaterial, e.PointerObjects.gameObject.layer);
                }
            }
        }
    }
}