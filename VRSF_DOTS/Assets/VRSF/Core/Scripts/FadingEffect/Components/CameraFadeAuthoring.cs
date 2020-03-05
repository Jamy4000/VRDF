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
        /// <summary>
        ///  How long, in seconds, the fade-in/fade-out animation should take
        /// </summary>
        [Tooltip("Speed of the \"blink\" animation (fading in and out upon teleport).")]
        [SerializeField] private float _fadingSpeed = 1;
        /// <summary>
        ///  How long, in seconds, the fade-in/fade-out animation should take
        /// </summary>
        [Tooltip("Whether a Fade In effect should take place when the OnSetupVRReady is called.")]
        [SerializeField] private bool _fadeInOnSetupVRReady = true;

        [Tooltip("Should we destroy this entity when the active scene is changed ?")]
        [SerializeField] private bool _destroyEntityOnSceneUnloaded = true;

        [Header("Required Fading Components")]
        [Tooltip("Plane Mesh used to fade")]
        [SerializeField] private GameObject _planePrefab;

        private void Start()
        {
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_planePrefab, settings);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var entity = entityManager.Instantiate(prefab);

            entityManager.AddComponentData(entity, new CameraFadeParameters()
            {
                FadingSpeed = _fadingSpeed,
                ShouldImmediatlyFadeIn = false,
                OldFadingSpeedFactor = _fadingSpeed,
                FadeInOnSceneLoaded = _fadeInOnSetupVRReady
            });

            if (_destroyEntityOnSceneUnloaded)
                OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(entityManager, entity, gameObject.scene.buildIndex, "CameraFadeAuthoring");

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "Camera Fade Entity");
#endif

            if (_fadeInOnSetupVRReady)
                new StartFadingInEvent();

            Destroy(gameObject);
        }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(CameraFadeAuthoring))]
    public class CameraFadeAuthoringInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.HelpBox("This GameObject will be destroyed on Start.", UnityEditor.MessageType.Warning);
        }
    }
#endif
}