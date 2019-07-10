using Unity.Entities;
using UnityEngine.Events;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// CBRA Component data coontaining the click events
    /// </summary>
    public struct CBRAClickEvents : IComponentData
    {
        public UnityEvent OnButtonStartClicking;
        public UnityEvent OnButtonStopClicking;
        public UnityEvent OnButtonIsClicking;
    }
}
