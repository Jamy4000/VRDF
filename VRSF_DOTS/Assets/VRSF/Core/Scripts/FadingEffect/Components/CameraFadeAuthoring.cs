using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.FadingEffect
{
    /// <summary>
    /// Component handling the fading effects. Need to be placed a canvas with an Image in front of the user's eyes
    /// </summary>
    public class CameraFadeAuthoring : MonoBehaviour
    {
        [Header("Fading Parameters")]
        [Tooltip("Speed of the fading effect. Higher values provoke faster fading effect.")]
        [SerializeField] private float _fadingSpeed = 0.7f;

        [Tooltip("Should we destroy this entity when the active scene is changed ?")]
        [SerializeField] private bool _destroyEntityOnSceneUnloaded = true;

        [Header("Required Fading Components")]
        [Tooltip("Plane Mesh used to fade")]
        [SerializeField] private GameObject _planePrefab;

        [Tooltip("Material used on the Fading Plane")]
        [SerializeField] private Material _fadeMaterial;


        [Tooltip("Whether a Fade In effect should take place when the OnSetupVRReady is called.")]
        [HideInInspector] [SerializeField] public bool FadeInOnSetupVRReady = true;

        [Tooltip("How long before we start the Fade In effect OnSetupVRReady.")]
        [HideInInspector] [SerializeField] public float TimeBeforeFirstFadeIn = 2.0f;

        private void Awake()
        {
            // Fetch World Settings, create entity prefab and fetch entityManager
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_planePrefab, settings);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // Create the new Fading Plane based on the prefab provided
            var entity = entityManager.Instantiate(prefab);

            // add the cameraFadeParameters component to the newly created entity, and inject the parameters provided in the inspector
            entityManager.AddComponentData(entity, new CameraFadeParameters()
            {
                FadingSpeed = _fadingSpeed,
                ShouldImmediatlyFadeIn = false,
                OldFadingSpeedFactor = _fadingSpeed
            });

            // Check if the user want a fade in on start
            CheckFadeInOnSetupVRReady();

            if (_destroyEntityOnSceneUnloaded)
                OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(entityManager, entity, gameObject.scene.buildIndex, "CameraFadeAuthoring");

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "Camera Fade Entity");
#endif

            Destroy(gameObject);


            /// <summary>
            /// Check if we want to fade in when setupVR is ready. Set the material alpha accordingly, and add a CameraFadeOnStart 
            /// component to the entity to activate the CameraFadeOnSetupVRReadySystem
            /// </summary>
            void CheckFadeInOnSetupVRReady()
            {
                // Reset material color alpha based on whether we want to fade in or not when SetupVR is ready
                var newColor = _fadeMaterial.color;

                if (FadeInOnSetupVRReady)
                {
                    newColor.a = 1.0f;
                    entityManager.AddComponentData(entity, new CameraFadeOnStart()
                    {
                        TimeBeforeFadeIn = TimeBeforeFirstFadeIn,
                        TimeSinceSceneLoaded = 0.0f
                    });
                }
                else
                {
                    newColor.a = 0.0f;
                }

                _fadeMaterial.color = newColor;
            }
        }
    }
}