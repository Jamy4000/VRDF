using System;
using Unity.Entities;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// CBRA Component data coontaining the touch events
    /// </summary>
    public struct CBRATouchEvents : IComponentData
    {
        public Action OnButtonStartTouching;
        public Action OnButtonIsTouching;
        public Action OnButtonStopTouching;

        public CBRATouchEvents(Action onStartTouching, Action onIsTouching, Action onStopTouching)
        {
            OnButtonStartTouching = onStartTouching;
            OnButtonIsTouching = onIsTouching;
            OnButtonStopTouching = onStopTouching;
        }
    }
}
