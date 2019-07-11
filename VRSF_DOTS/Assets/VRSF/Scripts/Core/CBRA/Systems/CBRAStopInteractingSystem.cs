using System;
using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// TODO
    /// </summary>
    public class CBRAStopInteractingSystem : ComponentSystem
    {
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            base.OnCreate();
            ButtonUntouchEvent.Listeners += OnButtonTouchedCallback;
            ButtonUnclickEvent.Listeners += OnButtonClickedCallback;
            _entityManager = World.Active.EntityManager;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ButtonUntouchEvent.Listeners -= OnButtonTouchedCallback;
            ButtonUnclickEvent.Listeners -= OnButtonClickedCallback;
        }

        protected override void OnUpdate() {}

        private void OnButtonTouchedCallback(ButtonUntouchEvent info)
        {
            Entities.ForEach((Entity entity, ref CBRAInteractionType cbraInteractionType) =>
            {
                Type buttonTouchInputType = CBRAInputTypeGetter.GetTypeFor(info.ButtonInteracting);
                if (CBRADelegatesHolder.TouchEvents.ContainsKey(entity) && _entityManager.HasComponent(entity, buttonTouchInputType))
                    CBRADelegatesHolder.TouchEvents[entity][ActionType.StopInteracting].Invoke();
            });
        }

        private void OnButtonClickedCallback(ButtonUnclickEvent info)
        {
            Entities.ForEach((Entity entity, ref CBRAInteractionType cbraInteractionType) =>
            {
                Type buttonClickInputType = CBRAInputTypeGetter.GetTypeFor(info.ButtonInteracting);
                if (CBRADelegatesHolder.ClickEvents.ContainsKey(entity) && _entityManager.HasComponent(entity, buttonClickInputType))
                    CBRADelegatesHolder.ClickEvents[entity][ActionType.StopInteracting].Invoke();
            });
        }
    }
}