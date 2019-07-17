using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// The base event to raise when one of the laserPointer changed its width
    /// </summary>
    public class OnLaserWidthChanged : EventCallbacks.Event<OnLaserWidthChanged>
    {
        public readonly ERayOrigin LaserOrigin;

        public readonly float NewWidth;

        public OnLaserWidthChanged(ERayOrigin laserOrigin, float newWidth) : base("The base event to raise when one of the laserPointer changed its width.")
        {
            LaserOrigin = laserOrigin;
            NewWidth = newWidth;

            FireEvent(this);
        }
    }
}