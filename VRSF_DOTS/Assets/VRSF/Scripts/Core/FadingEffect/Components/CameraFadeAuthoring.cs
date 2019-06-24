using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace VRSF.Core.FadingEffect
{
    /// <summary>
    /// Component handling the fading effects. Need to be placed a canvas with an Image in front of the user's eyes
    /// </summary>
    [RequiresEntityConversion]
    public class CameraFadeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Fading Parameters")]
        /// <summary>
        ///  How long, in seconds, the fade-in/fade-out animation should take
        /// </summary>
        [Tooltip("Duration of the \"blink\" animation (fading in and out upon teleport) in seconds.")]
        public float FadingSpeed = 1;
        
        [Header("Required Fading Components")]
        [Tooltip("Plane Mesh used to fade")]
        [SerializeField] private Mesh _mesh;
        [Tooltip("Plane Mesh used to fade")]
        [SerializeField] private Material _fadeMaterial;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            _fadeMaterial.color = new Color(_fadeMaterial.color.r, _fadeMaterial.color.g, _fadeMaterial.color.b, 1.0f);

            dstManager.AddSharedComponentData(entity, new RenderMesh()
            {
                mesh = _mesh,
                material = _fadeMaterial
            });
            
            dstManager.AddComponentData(entity, new CameraFade()
            {
                FadingSpeed = FadingSpeed,
                FadingInProgress = true,
                IsFadingIn = true,
                ShouldImmediatlyFadeIn = false,
                OldFadingSpeedFactor = -1.0f
            });

            dstManager.SetComponentData(entity, new Unity.Transforms.Translation
            {
                Value = new Unity.Mathematics.float3(0.0f, 0.0f, transform.localPosition.z + 0.3f)
            });

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            dstManager.SetName(entity, string.Format("Camera Fade Entity", entity.Index));
#endif
        }
    }

    public struct CameraFade : IComponentData
    {
        /// <summary>
        ///  How long, in seconds, the fade-in/fade-out animation should take
        /// </summary>
        public float FadingSpeed;

        public float OldFadingSpeedFactor;

        public bool IsFadingIn;

        /// <summary>
        /// Set via the StartFadingOutEvent Listeners and its ShouldFadeInWhenDone boolean
        /// </summary>
        public bool ShouldImmediatlyFadeIn;

        /// <summary>
        /// Whether the fading effect is currently in progress
        /// </summary>
        public bool FadingInProgress;
    }
} 