using Unity.Entities;

namespace VRSF.Core.FadingEffect
{
    public struct CameraFadeParameters : IComponentData
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

        /// <summary>
        /// Whether the fading effect should take place OnSetupVRReady
        /// </summary>
        public bool FadeInOnSceneLoaded;
    }
}
