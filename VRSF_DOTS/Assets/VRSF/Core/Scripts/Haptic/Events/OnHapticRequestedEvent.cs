using VRSF.Core.Controllers;
using VRSF.Core.Haptic;

/// <summary>
/// Event to call when you want to launch haptic in the controller of the user
/// </summary>
public class OnHapticRequestedEvent : EventCallbacks.Event<OnHapticRequestedEvent>
{
    /// <summary>
    /// The hand that is going to receive the Haptic feedback
    /// </summary>
    public readonly EHand Hand;

    /// <summary>
    /// Time in seconds the haptic impulse should take place
    /// </summary>
    public readonly float HapticDuration;

    /// <summary>
    /// The Amplitude of the Haptic impulse. WARNING : Only Supported on OCULUS
    /// </summary>
    public readonly float HapticAmplitude;

    /// <summary>
    /// Event to call when you want to launch haptic in the controller of the user.
    /// </summary>
    /// <param name="hand">The hand that is gonna receive the Haptic feedback</param>
    /// <param name="hapticDuration">Time in seconds the haptic impulse should take place</param>
    /// <param name="hapticAmplitude">The Amplitude of the Haptic impulse. WARNING : Only Supported on OCULUS</param>
    public OnHapticRequestedEvent(EHand hand, EHapticDuration hapticDuration = EHapticDuration.MEDIUM, EHapticAmplitude hapticAmplitude = EHapticAmplitude.MEDIUM) : base("Event to call when you want to launch haptic in the controller of the user.")
    {
        Hand = hand;
        HapticDuration = GetBaseDuration(hapticDuration);
        HapticAmplitude = GetBaseAmplitude(hapticAmplitude);
        FireEvent(this);
    }

    /// <summary>
    /// Event to call when you want to launch haptic in the controller of the user.
    /// </summary>
    /// <param name="hand">The hand that is gonna receive the Haptic feedback</param>
    /// <param name="hapticDuration">Time in seconds the haptic impulse should take place</param>
    /// <param name="hapticAmplitude">The Amplitude of the Haptic impulse. WARNING : Only Supported on OCULUS</param>
    public OnHapticRequestedEvent(EHand hand, float hapticDuration = 0.5f, float hapticAmplitude = 1.0f) : base("Event to call when you want to launch haptic in the controller of the user.")
    {
        Hand = hand;
        HapticDuration = hapticDuration;
        HapticAmplitude = hapticAmplitude;
        FireEvent(this);
    }

    /// <summary>
    /// Change EHapticAmpitude into its corresponding amplitude level
    /// </summary>
    /// <param name="hapticAmplitude">The EHapticAmplitude we specified in the event and we want as float</param>
    /// <returns>The corresponding amplitude level</returns>
    private float GetBaseAmplitude(EHapticAmplitude hapticAmplitude)
    {
        switch (hapticAmplitude)
        {
            case EHapticAmplitude.LIGHT:
                return 0.1f;
            case EHapticAmplitude.MEDIUM:
                return 0.5f;
            case EHapticAmplitude.HARD:
                return 1.0f;
            default:
                throw new System.Exception();
        };
    }

    /// <summary>
    /// Change EHapticDuration into its corresponding duration
    /// </summary>
    /// <param name="hapticDuration">The EHapticDuration we specified in the event and we want as float</param>
    /// <returns>The corresponding duration in seconds</returns>
    private float GetBaseDuration(EHapticDuration hapticDuration)
    {
        switch (hapticDuration)
        {
            case EHapticDuration.SHORT:
                return 0.25f;
            case EHapticDuration.MEDIUM:
                return 1.0f;
            case EHapticDuration.LONG:
                return 2.0f;
            default:
                throw new System.Exception();
        };
    }
}