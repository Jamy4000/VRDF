namespace VRSF.Core.FadingEffect
{
    public class StartFadingInEvent : EventCallbacks.Event<StartFadingInEvent>
    {
        /// <summary>
        /// If you want to override the general speed parameter for the fading effect.
        /// </summary>
        public readonly float SpeedOverride;

        public StartFadingInEvent(float speedOverride = -1.0f) : base("Event called when we want to start a fade in effect")
        {
            SpeedOverride = speedOverride;
            FireEvent(this);
        }
    }
}