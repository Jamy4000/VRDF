namespace VRDF.Core.Haptic
{
    /// <summary>
    /// The magnitude of the haptic event. This value must be between 0.0 and 1.0.
    /// </summary>
    public enum EHapticAmplitude
    {
        /// <summary>
        /// Correspond to 0.1f
        /// </summary>
        LIGHT,

        /// <summary>
        /// Correspond to 0.5f
        /// </summary>
        MEDIUM,

        /// <summary>
        /// Correspond to 1.0f
        /// </summary>
        HARD
    }
}