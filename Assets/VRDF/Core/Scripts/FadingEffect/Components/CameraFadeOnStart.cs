using Unity.Entities;

namespace VRDF.Core.FadingEffect
{
    /// <summary>
    /// Used when we want a FadeIn once SetupVR is Ready
    /// </summary>
    public struct CameraFadeOnStart : IComponentData
    {
        /// <summary>
        ///  How long should we wait before starting the FadeIn effect on SetupVR Ready
        /// </summary>
        public float TimeBeforeFadeIn;

        /// <summary>
        ///  Since how long was the scene loaded ?
        /// </summary>
        public float TimeSinceSceneLoaded;
    }
}
