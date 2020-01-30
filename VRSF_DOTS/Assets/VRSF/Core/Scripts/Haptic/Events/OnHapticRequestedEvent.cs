namespace VRSF.Core.Controllers.Haptic
{
    /// <summary>
    /// Event to call when you want to launch haptic in the controller of the user
    /// </summary>
    public class OnHapticRequestedEvent : EventCallbacks.Event<OnHapticRequestedEvent>
    {
        /// <summary>
        /// The hand that is gonna receive the Haptic feedback
        /// </summary>
        public readonly EHand Hand;

        /// <summary>
        /// Time in microseconds the haptic impulse should take place
        /// </summary>
        public readonly float HapticDuration;

        /// <summary>
        /// The Amplitude of the Haptic impulse. WARNING : Only Supported on OCULUS
        /// </summary>
        public readonly float HapticAmplitude;

        public OnHapticRequestedEvent(EHand hand, EHapticDuration hapticDuration, EHapticAmplitude hapticAmplitude = EHapticAmplitude.MEDIUM) : base("Event to call when you want to launch haptic in the controller of the user.")
        {
            Hand = hand;
            HapticDuration = GetBaseDuration(hapticDuration);
            HapticAmplitude = GetBaseAmplitude(hapticAmplitude);
            FireEvent(this);
        }

        public OnHapticRequestedEvent(EHand hand, float hapticDuration = 1.0f, float hapticAmplitude = 1.0f) : base("Event to call when you want to launch haptic in the controller of the user.")
        {
            Hand = hand;
            HapticDuration = hapticDuration;
            HapticAmplitude = hapticAmplitude;
            FireEvent(this);
        }

        private float GetBaseAmplitude(EHapticAmplitude hapticAmplitude)
        {
            switch (hapticAmplitude)
            {
                case EHapticAmplitude.LIGHT:
                    return 0.1f;
                case EHapticAmplitude.MEDIUM:
                    return 0.5f;
                default:
                    return 1.0f;
            }
        }

        private float GetBaseDuration(EHapticDuration hapticDuration)
        {
            switch (hapticDuration)
            {
                case EHapticDuration.SHORT:
                    return 0.25f;
                case EHapticDuration.MEDIUM:
                    return 1.0f;
                default:
                    return 2.0f;
            }
        }
    }
}