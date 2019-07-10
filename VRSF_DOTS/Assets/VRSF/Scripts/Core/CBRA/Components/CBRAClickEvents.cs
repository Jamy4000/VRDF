using System;
using Unity.Entities;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// CBRA Component data coontaining the click events
    /// </summary>
    public struct CBRAClickEvents : IComponentData
    {
        public Action OnButtonStartClicking;
        public Action OnButtonIsClicking;
        public Action OnButtonStopClicking;

        public CBRAClickEvents(Action onStartClicking, Action onIsClicking, Action onStopClicking)
        {
            OnButtonStartClicking = onStartClicking;
            OnButtonIsClicking = onIsClicking;
            OnButtonStopClicking = onStopClicking;
        }
    }
}
