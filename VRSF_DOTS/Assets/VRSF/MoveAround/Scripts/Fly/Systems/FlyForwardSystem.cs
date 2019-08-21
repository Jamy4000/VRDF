using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Calculate if the user is flying forward or backward and init some values.
    /// </summary>
    public class FlyForwardSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, ref FlyDirection direction, ref TouchpadInputCapture tic, ref BaseInputCapture bic, ref ControllersInteractionType cit) =>
            {
                // If user just started to press/touch the thumbstick
                if (tic.ThumbPosition.y != 0.0f && InteractionChecker.IsInteracting(bic, cit))
                {
                    direction.FlightDirection = tic.ThumbPosition.y;

                    if (!EntityManager.HasComponent(e, typeof(IsFlying)))
                        PostUpdateCommands.AddComponent(e, new IsFlying());
                    if (EntityManager.HasComponent(e, typeof(IsDecelerating)))
                        PostUpdateCommands.RemoveComponent<IsDecelerating>(e);
                }
                else if (EntityManager.HasComponent(e, typeof(IsFlying)))
                {
                    UnityEngine.Debug.Log("Has IS Flying");
                    PostUpdateCommands.RemoveComponent<IsFlying>(e);
                    PostUpdateCommands.AddComponent(e, new IsDecelerating());
                }
            });
        }
    }
}