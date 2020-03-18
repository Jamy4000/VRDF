using Unity.Entities;
using VRDF.Core.Inputs;
using VRDF.Core.VRInteractions;

namespace VRDF.MoveAround.Fly
{
    /// <summary>
    /// Calculate if the user is flying forward or backward and init some values.
    /// </summary>
    public class FlyForwardSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, ref FlyDirection direction, ref TouchpadInputCapture tic, ref InteractionThumbPosition itp, ref BaseInputCapture bic, ref ControllersInteractionType cit) =>
            {
                // If user just started to press/touch the thumbstick
                if (InteractionChecker.IsInteractingTouchpad(bic, cit, itp, tic, true, false))
                {
                    direction.FlightDirection = tic.ThumbPosition.y;

                    if (!EntityManager.HasComponent(e, typeof(IsFlying)))
                        PostUpdateCommands.AddComponent(e, new IsFlying());
                    if (EntityManager.HasComponent(e, typeof(IsDecelerating)))
                        PostUpdateCommands.RemoveComponent<IsDecelerating>(e);
                }
                else if (EntityManager.HasComponent(e, typeof(IsFlying)))
                {
                    PostUpdateCommands.RemoveComponent<IsFlying>(e);
                    PostUpdateCommands.AddComponent(e, new IsDecelerating());
                }
            });
        }
    }
}