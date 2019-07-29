using Unity.Entities;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// If the user is pressing/touching the flying button, we handle the acceleration
    /// </summary>
    [UpdateAfter(typeof(FlyAccelerationCalculationsSystem))]
    public class FlyDecelerationCalculationsSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll(typeof(IsDecelerating)).ForEach((Entity e, ref FlySpeed speed, ref FlyDirection direction, ref FlyAcceleration acceleration, ref FlyDeceleration deceleration) =>
            {
                if (deceleration.SlowDownTimer > 0.0f)
                    deceleration.SlowDownTimer = deceleration.DecelerationEffectFactor != 0.0f ? deceleration.SlowDownTimer - (UnityEngine.Time.deltaTime / deceleration.DecelerationEffectFactor) : 0.0f;

                speed.CurrentFlightVelocity = speed.GetSpeed() * deceleration.SlowDownTimer;

                if (speed.CurrentFlightVelocity <= 0.0f)
                {
                    PostUpdateCommands.RemoveComponent<IsDecelerating>(e);
                    deceleration.SlowDownTimer = 0.0f;
                }
            });
        }
    }
}