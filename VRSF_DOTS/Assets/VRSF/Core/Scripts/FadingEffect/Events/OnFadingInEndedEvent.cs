namespace VRSF.Core.FadingEffect
{
    public class OnFadingInEndedEvent : EventCallbacks.Event<OnFadingInEndedEvent>
    {
        public OnFadingInEndedEvent() : base("Event called when the fade in effect is done")
        {
            FireEvent(this);
        }
    }
}