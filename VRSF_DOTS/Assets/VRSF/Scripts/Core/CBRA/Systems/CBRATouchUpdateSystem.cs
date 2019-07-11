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
                All = new[] { ComponentType.ReadWrite<CBRAInteractionType>(), ComponentType.ReadWrite<BaseInputCapture>(), },
            });
        }

        protected override void OnUpdate()
        {
            var cbraInteractions = _cbraQuery.ToComponentDataArray<CBRAInteractionType>(Unity.Collections.Allocator.TempJob);
            var baseInput = _cbraQuery.ToComponentDataArray<BaseInputCapture>(Unity.Collections.Allocator.TempJob);

            for (int i = 0; i < cbraInteractions.Length; i++)
            {
                if ((cbraInteractions[i].InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH && baseInput[i].IsTouching)
                {
                    CBRADelegatesHolder.TouchEvents[cbraInteractions[i]][ActionType.IsInteracting].Invoke();
                    UnityEngine.Debug.Log("cbra touching");
                }
            }

            cbraInteractions.Dispose();
            baseInput.Dispose();
        }
    }
}