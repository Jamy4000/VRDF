using UnityEngine;
using VRSF.Core.VRInteraction;
using Unity.Entities;
using System.Collections.Generic;
using VRSF.Core.SetupVR;
using VRSF.Core.Inputs;
using VRSF.Core.Interactions;
using VRSF.Core.Raycast;
using Unity.Rendering;

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
                    typeof(ParabolPadPrefabs),
                    typeof(ParabolPointsParameters),
                    typeof(ParabolCalculations),
                    typeof(RenderMesh),
                    typeof(GeneralTeleportParameters),
                    typeof(TeleportNavMesh)
                );

                var teleporterEntity = entityManager.CreateEntity(archetype);

                // Setting up Interactions
                if (!TeleporterSetupHelper.SetupInteractions(ref entityManager, ref teleporterEntity, interactionParameters))
                {
                    entityManager.DestroyEntity(teleporterEntity);
                    Destroy(gameObject);
                    return;
                }

                // Setting up Raycasting
                if (!TeleporterSetupHelper.SetupRaycast(ref entityManager, ref teleporterEntity, interactionParameters, PointCount))
                {
                    entityManager.DestroyEntity(teleporterEntity);
                    Destroy(gameObject);
                    return;
                }

                // Setting up General Teleporter Stuffs
                TeleporterSetupHelper.SetupTeleportStuffs(ref entityManager, ref teleporterEntity, GetComponent<GeneralTeleportAuthoring>());

                // Setup Specific curve teleporter calculations stuffs
                entityManager.SetComponentData(teleporterEntity, new CurveTeleporterCalculations
                {
                    Acceleration = Acceleration,
                    InitialVelocity = InitialVelocity
                });

                entityManager.SetComponentData(teleporterEntity, new ParabolCalculations { Origin = GetComponent<VRInteractionAuthoring>().ButtonHand });

                entityManager.SetComponentData(teleporterEntity, new ParabolPointsParameters
                {
                    PointCount = PointCount,
                    PointSpacing = PointSpacing
                });

                // Setup Specific curve teleporter rendering stuffs
                var parabolMesh = new Mesh
                {
                    name = "Parabolic Pointer",
                    vertices = new Vector3[0],
                    triangles = new int[0]
                };
                parabolMesh.MarkDynamic();

                // This rendermesh is only here to store the mesh, material and layer of the curve teleporter and draw it later in a system
                entityManager.SetSharedComponentData(teleporterEntity, new RenderMesh
                {
                    mesh = parabolMesh,
                    material = GraphicMaterial,
                    castShadows = UnityEngine.Rendering.ShadowCastingMode.Off,
                    layer = 0,
                    receiveShadows = false,
                    subMesh = 0
                });

                entityManager.SetComponentData(teleporterEntity, new CurveTeleporterRendering
                {
                    //GraphicMaterial = GraphicMaterial,
                    GraphicThickness = GraphicThickness
                });

                entityManager.SetComponentData(teleporterEntity, new ParabolPadPrefabs
                {
                    SelectionPadPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(SelectionPadPrefab, World.Active),
                    InvalidPadPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(InvalidPadPrefab, World.Active)
                });

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(teleporterEntity, "Curve Teleporter Entity");
#endif

                // Create parabol points as entities, as List can't be used in ComponentData
                var pointArchetype = entityManager.CreateArchetype
                (
                    typeof(Unity.Transforms.Translation),
                    typeof(ParabolPointParent),
                    typeof(ParabolPointTag)
                );

                for (int i = 0; i < PointCount; i++)
                {
                    var parabolPoint = entityManager.CreateEntity(pointArchetype);
                    entityManager.SetSharedComponentData(parabolPoint, new ParabolPointParent { TeleporterEntityIndex = teleporterEntity.Index });
#if UNITY_EDITOR
                    // Set it's name in Editor Mode for the Entity Debugger Window
                    entityManager.SetName(parabolPoint, "Curve Teleporter Point " + i);
#endif
                }

            }

            Destroy(gameObject);
        }
    }
}