using Unity.Entities;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Used to work on identity that are only used in the CBRA Systems.
    /// </summary>
    public struct CBRAEventComponent : IComponentData 
    {
        public bool HasCheckedStartClickingEvent;
        public bool HasCheckedStopClickingEvent;

        public bool HasCheckedStartTouchingEvent;
        public bool HasCheckedStopTouchingEvent;
    }
}
