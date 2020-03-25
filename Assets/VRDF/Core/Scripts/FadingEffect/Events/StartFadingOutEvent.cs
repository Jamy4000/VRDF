/// <summary>
/// Event called when we want to start a fade out effect
/// </summary>
public class StartFadingOutEvent : EventCallbacks.Event<StartFadingOutEvent>
{
    /// <summary>
    /// Whether we wanna fade in directly after the fade out is done.
    /// If this option is at false, you may wanna think about calling the StartFadingInEvent when you need it.
    /// </summary>
    public readonly bool ShouldFadeInWhenDone;

    /// <summary>
    /// If you want to override the general speed parameter for the fading effect.
    /// </summary>
    public readonly float SpeedOverride;

    /// <summary>
    /// Event called when we want to start a fade out effect
    /// </summary>
    /// <param name="shouldFadeInWhenDone">Whether we wanna fade in directly after the fade out is done.</param>
    /// <param name="speedOverride">If you want to override the general speed parameter for the fading effect.</param>
    public StartFadingOutEvent(bool shouldFadeInWhenDone = false, float speedOverride = -1.0f) : base("Event called when we want to start a fade out effect")
    {
        ShouldFadeInWhenDone = shouldFadeInWhenDone;
        SpeedOverride = speedOverride;

        FireEvent(this);
    }
}