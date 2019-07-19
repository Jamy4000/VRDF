using UnityEngine;
using VRSF.Core.VRInteraction;
using Unity.Entities;
using System.Collections.Generic;
using VRSF.Core.SetupVR;
using VRSF.Core.Inputs;
using VRSF.Core.Interactions;
using VRSF.Core.Raycast;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Class use to author the Parabolic/Bezier teleporter
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    [RequireComponent(typeof(GeneralTeleportAuthoring), typeof(VRInteractionAuthoring))]
    public class CurveTeleporterAuthoring : MonoBehaviour
    {
        [Header("Parabola Trajectory")]
        [Tooltip("Initial velocity of the parabola, in local space.")]
        public Vector3 InitialVelocity = Vector3.forward * 10f;
        [Tooltip("World-space \"acceleration\" of the parabola.  This effects the falloff of the curve.")]
        public Vector3 Acceleration = Vector3.up * -9.8f;

        [Header("Parabola Mesh Properties")]
        [Tooltip("Number of points on the parabola mesh.  Greater point counts lead to a higher poly/smoother mesh.")]
        public int PointCount = 100;
        [Tooltip("Approximate spacing between each of the points on the parabola mesh.")]
        public float PointSpacing = 0.5f;
        [Tooltip("Thickness of the parabola mesh")]
        public float GraphicThickness = 0.2f;
        [Tooltip("Material to use to render the parabola mesh")]
        public Material GraphicMaterial;

        [Header("Selection Pad Properties")]
        [Tooltip("Prefab to use as the selection pad when the player is pointing at a valid teleportable surface.")]
        public GameObject SelectionPadPrefab;
        [Tooltip("Prefab to use as the invalid pad when the player is pointing at an invalid teleportable surface.")]
        public GameObject InvalidPadPrefab;

#if UNITY_EDITOR
        // Only used for the OnDrawGizmos method
        [System.NonSerialized] public List<Vector3> ParabolaPoints_Gizmo;
#endif

        private void Awake()
        {
            VRInteractionAuthoring interactionParameters = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device using this CBRA
            if ((interactionParameters.DeviceUsingCBRA & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                var entityManager = World.Active.EntityManager;
                
                var archetype = entityManager.CreateArchetype
                (
                    typeof(BaseInputCapture),
                    typeof(ControllersInteractionType),
                    typeof(VRRaycastOutputs),
                    typeof(VRRaycastOrigin),
                    typeof(VRRaycastParameters),
                    typeof(CurveTeleporterCalculations),
                    typeof(CurveTeleporterRendering),
                    typeof(ParabolObjects),
                    typeof(ParabolPointParameter),
                    typeof(ParabolCalculations),
                    typeof(GeneralTeleportParameters),
                    typeof(TeleportNavMesh)
                );

                var entity = entityManager.CreateEntity(archetype);

                // Setting up Interactions
                if (!TeleporterSetupHelper.SetupInteractions(ref entityManager, ref entity, interactionParameters))
                {
                    entityManager.DestroyEntity(entity);
                    Destroy(gameObject);
                    return;
                }

                // Setting up Raycasting
                if (!TeleporterSetupHelper.SetupRaycast(ref entityManager, ref entity, interactionParameters, PointCount))
                {
                    entityManager.DestroyEntity(entity);
                    Destroy(gameObject);
                    return;
                }

                // Setting up General Teleporter Stuffs
                TeleporterSetupHelper.SetupTeleportStuffs(ref entityManager, ref entity, GetComponent<GeneralTeleportAuthoring>());

                // Setup Specific curve teleporter stuffs
                entityManager.SetComponentData(entity, new CurveTeleporterCalculations
                {
                    Acceleration = Acceleration,
                    InitialVelocity = InitialVelocity
                });

                var parabolaMesh = new Mesh
                {
                    name = "Parabolic Pointer",
                    vertices = new Vector3[0],
                    triangles = new int[0]
                };
                parabolaMesh.MarkDynamic();

                entityManager.SetComponentData(entity, new CurveTeleporterRendering
                {
                    //_parabolaMesh = parabolaMesh,
                    //GraphicMaterial = GraphicMaterial,
                    GraphicThickness = GraphicThickness
                });

                entityManager.SetComponentData(entity, new ParabolPointParameter
                {
                    PointCount = PointCount,
                    PointSpacing = PointSpacing
                });

                entityManager.SetComponentData(entity, new ParabolObjects
                {
                    SelectionPadPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(SelectionPadPrefab, World.Active),
                    InvalidPadPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(InvalidPadPrefab, World.Active)
                });

                entityManager.SetComponentData(entity, new ParabolCalculations());

                entityManager.AddSharedComponentData(entity, new ParabolPoints
                {
                    ParabolaPoints = new Unity.Collections.NativeList<Unity.Mathematics.float3>(PointCount, Unity.Collections.Allocator.Persistent)
                });

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, "Curve Teleporter Entity");
#endif
            }

            Destroy(gameObject);
        }
    }
}