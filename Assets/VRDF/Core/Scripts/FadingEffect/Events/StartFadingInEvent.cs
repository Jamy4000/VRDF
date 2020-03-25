/// <summary>
/// Event called when we want to start a fade in effect
/// </summary>
public class StartFadingInEvent : EventCallbacks.Event<StartFadingInEvent>
{
    /// <summary>
    /// If you want to override the general speed parameter for the fading effect.
    /// </summary>
    public readonly float SpeedOverride;

    /// <summary>
    /// Event called when we want to start a fade in effect
    /// </summary>
    /// <param name="speedOverride">If you want to override the general speed parameter for the fading effect.</param>
    public StartFadingInEvent(float speedOverride = -1.0f) : base("Event called when we want to start a fade in effect")
    {
        SpeedOverride = speedOverride;
        FireEvent(this);
    }
}