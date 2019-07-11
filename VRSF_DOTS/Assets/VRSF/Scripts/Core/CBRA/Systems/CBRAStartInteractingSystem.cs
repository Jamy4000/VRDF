using System;
using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// TODO
    /// </summary>
    public class CBRAStartInteractingSystem : ComponentSystem
    {
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            base.OnCreate();
            ButtonTouchEvent.Listeners += OnButtonTouchedCallback;
            ButtonClickEvent.Listeners += OnButtonClickedCallback;
            _entityManager = World.Active.EntityManager;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ButtonTouchEvent.Listeners -= OnButtonTouchedCallback;
            ButtonClickEvent.Listeners -= OnButtonClickedCallback;
        }

        protected override void OnUpdate() {}

        private void OnButtonTouchedCallback(ButtonTouchEvent info)
        {
            Entities.ForEach((Entity entity, ref CBRAInteractionType cbraInteractionType) =>
            {
                Type buttonTouchInputType = CBRAInputTypeGetter.GetTypeFor(info.ButtonInteracting);
                if (CBRADelegatesHolder.TouchEvents.ContainsKey(entity) && _entityManager.HasComponent(entity, buttonTouchInputType))
                    CBRADelegatesHolder.TouchEvents[entity][ActionType.StartInteracting].Invoke();
            });
        }

        private void OnButtonClickedCallback(ButtonClickEvent info)
        {
            Entities.ForEach((Entity entity, ref CBRAInteractionType cbraInteractionType) =>
            {
                Type buttonClickInputType = CBRAInputTypeGetter.GetTypeFor(info.ButtonInteracting);
                if (CBRADelegatesHolder.ClickEvents.ContainsKey(entity) && _entityManager.HasComponent(entity, buttonClickInputType))
                    CBRADelegatesHolder.ClickEvents[entity][ActionType.StartInteracting].Invoke();
            });
        }
    }
}