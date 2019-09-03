using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// The base event to raise when one of the laserPointer changed its length
    /// </summary>
    public class OnLaserLengthChanged : EventCallbacks.Event<OnLaserLengthChanged>
    {
        public readonly ERayOrigin LaserOrigin;

        public readonly Vector3 NewEndPos;

        public OnLaserLengthChanged(ERayOrigin lasersOrigin, Vector3 newEndPos) : base("The base event to raise when one of the laserPointer changed its length.")
        {
            LaserOrigin = lasersOrigin;
            NewEndPos = newEndPos;

            FireEvent(this);
        }
    }
}