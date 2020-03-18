/// <summary>
/// Event called when the fade out effect is done
/// </summary>
public class OnFadingOutEndedEvent : EventCallbacks.Event<OnFadingOutEndedEvent>
{
    /// <summary>
    /// Event called when the fade out effect is done
    /// </summary>
    public OnFadingOutEndedEvent() : base("Event called when the fade out effect is done")
    {
        FireEvent(this);
    }
}