using UnityEngine;
using VRSF.Core.VRInteractions;
using Unity.Entities;
using VRSF.Core.SetupVR;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;
using Unity.Rendering;
using VRSF.Core.Utils;

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
        [Tooltip("GameObject to use as the selection pad when the player is pointing at a valid teleportable surface. Turned to Entity at runtime.")]
        public GameObject SelectionPad;
        [Tooltip("GameObject to use as the invalid pad when the player is pointing at an invalid teleportable surface. Turned to Entity at runtime.")]
        public GameObject InvalidPad;

        [Header("Other Parameters")]
        [Tooltip("Should we destroy this entity when the active scene is changed ?.")]
        [SerializeField] private bool _destroyOnSceneUnloaded = true;

        private void Awake()
        {
            VRInteractionAuthoring interactionParameters = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device using this CBRA
            if ((interactionParameters.DeviceUsingFeature & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                var archetype = entityManager.CreateArchetype
                (
                    typeof(BaseInputCapture),
                    typeof(ControllersInteractionType),
                    typeof(VRRaycastOutputs),
                    typeof(VRRaycastOrigin),
                    typeof(VRRaycastParameters),
                    typeof(CurveTeleporterCalculations),
                    typeof(CurveTeleporterRendering),
                    typeof(ParabolPadsEntities),
                    typeof(ParabolPointsParameters),
                    typeof(ParabolCalculations),
                    typeof(RenderMesh),
                    typeof(GeneralTeleportParameters),
                    typeof(TeleportNavMesh)
                );

                var teleporterEntity = entityManager.CreateEntity(archetype);

                // Setting up Interactions
                if (!InteractionSetupHelper.SetupInteractions(ref entityManager, ref teleporterEntity, interactionParameters))
                {
                    entityManager.DestroyEntity(teleporterEntity);
                    Destroy(gameObject);
                    return;
                }

                // Setting up Raycasting
                if (!TeleporterSetupHelper.SetupRaycast(ref entityManager, ref teleporterEntity, interactionParameters, 10))
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

                // Create the valid and Invalid Pads
                var selectionPad = GameObjectConversionUtility.ConvertGameObjectHierarchy(SelectionPad, World.DefaultGameObjectInjectionWorld);
                var invalidPad = GameObjectConversionUtility.ConvertGameObjectHierarchy(InvalidPad, World.DefaultGameObjectInjectionWorld);

                entityManager.SetEnabled(selectionPad, false);
                entityManager.SetEnabled(invalidPad, false);

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(selectionPad, "Curve Teleporter Selection Pad");
                entityManager.SetName(invalidPad, "Curve Teleporter Invalid Pad");
#endif

                entityManager.SetComponentData(teleporterEntity, new ParabolPadsEntities
                {
                    SelectionPadInstance = selectionPad,
                    InvalidPadInstance = invalidPad
                });

                if (_destroyOnSceneUnloaded)
                {
                    entityManager.AddComponentData(selectionPad, new DestroyOnSceneUnloaded { SceneIndex = gameObject.scene.buildIndex });
                    entityManager.AddComponentData(invalidPad, new DestroyOnSceneUnloaded { SceneIndex = gameObject.scene.buildIndex });
                    entityManager.AddComponentData(teleporterEntity, new DestroyOnSceneUnloaded { SceneIndex = gameObject.scene.buildIndex });
                }

                Destroy(SelectionPad);
                Destroy(InvalidPad);

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(teleporterEntity, "Curve Teleporter Entity");
                entityManager.SetName(selectionPad, "Curve Teleporter Valid Pad Entity");
                entityManager.SetName(invalidPad, "Curve Teleporter Invalid Pad Entity");
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
                    if (_destroyOnSceneUnloaded)
                        entityManager.AddComponentData(parabolPoint, new DestroyOnSceneUnloaded { SceneIndex = gameObject.scene.buildIndex });
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