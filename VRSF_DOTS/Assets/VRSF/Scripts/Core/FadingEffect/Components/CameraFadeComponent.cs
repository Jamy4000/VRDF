using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.FadingEffect
{
    /// <summary>
    /// Component handling the fading effects. Need to be placed a canvas with an Image in front of the user's eyes
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class CameraFadeComponent : MonoBehaviour
    {
        /// How long, in seconds, the fade-in/fade-out animation should take
        [Tooltip("Duration of the \"blink\" animation (fading in and out upon teleport) in seconds.")]
        public float FadingSpeed = 1;

        public UnityEngine.UI.Image FadingImage { get; internal set; }

        [HideInInspector] public float OldFadingSpeedFactor;

        [HideInInspector] public bool IsFadingIn;

        /// <summary>
        /// Set via the StartFadingOutEvent Listeners and its ShouldFadeInWhenDone boolean
        /// </summary>
        [HideInInspector] public bool ShouldImmediatlyFadeIn;

        /// <summary>
        /// Whether the fading effect is currently in progress
        /// </summary>
        [HideInInspector] public bool FadingInProgress;
    }

    //public struct CameraFade : IComponentData
    //{
    //    /// <summary>
    //    ///  How long, in seconds, the fade-in/fade-out animation should take
    //    /// </summary>
    //    public float FadingSpeed;

    //    public float OldFadingSpeedFactor;

    //    public bool IsFadingIn;

    //    /// <summary>
    //    /// Set via the StartFadingOutEvent Listeners and its ShouldFadeInWhenDone boolean
    //    /// </summary>
    //    public bool ShouldImmediatlyFadeIn;

    //    /// <summary>
    //    /// Whether the fading effect is currently in progress
    //    /// </summary>
    //    public bool FadingInProgress;
    //}
} 