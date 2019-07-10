using System;
using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the touch on all controllers button EXCEPT the touchpad
    /// </summary>
    public class CBRATouchUpdateSystem : ComponentSystem
    {
        private EntityQuery _cbraQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._cbraQuery = GetEntityQuery(new EntityQueryDesc
            {
                Any = Array.Empty<ComponentType>(),
                // We need to check the position of the thumb for the touchpad, so we use a seperate system for that
                None = new[] { ComponentType.ReadWrite<TouchpadInputCapture>() },
                All = new[] { ComponentType.ReadWrite<CBRATouchEvents>(), ComponentType.ReadWrite<BaseInputCapture>(), },
            });
        }

        protected override void OnUpdate()
        {
            var touchEvents = _cbraQuery.ToComponentDataArray<CBRATouchEvents>(Unity.Collections.Allocator.Temp);
            var baseInput = _cbraQuery.ToComponentDataArray<BaseInputCapture>(Unity.Collections.Allocator.Temp);

            for (int i = 0; i < touchEvents.Length; i++)
            {
                if (baseInput[i].IsTouching)
                    touchEvents[i].OnButtonIsTouching.Invoke();
            }
        }
    }
}