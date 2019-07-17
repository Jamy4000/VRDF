namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Event to call when you want to change the dominant hand of the user when using GearVR or Oculus Go
    /// </summary>
    public class ChangeDominantHandEvent : EventCallbacks.Event<ChangeDominantHandEvent>
    {
        public readonly EHand NewDominantHand;

        public ChangeDominantHandEvent(EHand newDominantHand) : base("Event to call when you want to change the dominant hand of the user when using GearVR or Oculus Go")
        {
            NewDominantHand = newDominantHand;
            FireEvent(this);
        }
    }
}