using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// CBRA Component data coontaining the touch events
    /// </summary>
    public struct CBRATouchEvents : IComponentData
    {
        public UnityEvent OnButtonStartTouching;
        public UnityEvent OnButtonStopTouching;
        public UnityEvent OnButtonIsTouching;
    }
}
