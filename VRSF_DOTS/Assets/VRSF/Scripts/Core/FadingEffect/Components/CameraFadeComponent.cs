using UnityEngine;

namespace VRSF.Core.FadingEffect
{
    /// <summary>
    /// Component handling the fading effects. Need to be placed a canvas with an Image in front of the user's eyes
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(UnityEngine.UI.Image))]
    public class CameraFadeComponent : MonoBehaviour
    {
        /// How long, in seconds, the fade-in/fade-out animation should take
        [Tooltip("Duration of the \"blink\" animation (fading in and out upon teleport) in seconds.")]
        public float FadingSpeed = 1;

        [HideInInspector] public float _OldFadingSpeedFactor = -1.0f;

        [HideInInspector] public bool _IsFadingIn;

        /// <summary>
        /// Set via the StartFadingOutEvent Listeners and its ShouldFadeInWhenDone boolean
        /// </summary>
        [HideInInspector] public bool _ShouldImmediatlyFadeIn;

        /// <summary>
        /// Whether the fading effect is currently in progress
        /// </summary>
        [HideInInspector] public bool FadingInProgress = false;
    }
} 