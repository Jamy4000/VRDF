namespace VRSF.Core.FadingEffect
{
    public class OnFadingOutEndedEvent : EventCallbacks.Event<OnFadingOutEndedEvent>
    {
        public OnFadingOutEndedEvent() : base("Event called when the fade out effect is done")
        {
            FireEvent(this);
        }
    }
}