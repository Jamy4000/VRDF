using Unity.Entities;

namespace VRSF.Core.FadingEffect
{
    public struct CameraFadeParameters : IComponentData
    {
        /// <summary>
        ///  How long, in seconds, the fade-in/fade-out animation should take
        /// </summary>
        public float FadingSpeed;

        /// <summary>
        /// Used for whe we override the fading speed factor
        /// </summary>
        public float OldFadingSpeedFactor;

        /// <summary>
        /// Set via the StartFadingOutEvent Listeners and its ShouldFadeInWhenDone boolean
        /// </summary>
        public bool ShouldImmediatlyFadeIn;
    }
}
