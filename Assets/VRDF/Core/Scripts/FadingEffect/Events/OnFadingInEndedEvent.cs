/// <summary>
/// Event called when the fade in effect is done
/// </summary>
/// <summary>
/// Event called when the fade in effect is done
/// </summary>
public class OnFadingInEndedEvent : EventCallbacks.Event<OnFadingInEndedEvent>
{
    /// <summary>
    /// Event called when the fade in effect is done
    /// </summary>
    public OnFadingInEndedEvent() : base("Event called when the fade in effect is done")
    {
        FireEvent(this);
    }
}