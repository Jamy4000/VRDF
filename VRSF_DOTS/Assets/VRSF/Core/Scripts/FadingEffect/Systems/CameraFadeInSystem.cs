using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeInSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            StartFadingInEvent.Listeners += StartFadeIn;
            OnFadingInEndedEvent.Listeners += OnFadeInEnded;
            base.OnCreate();
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StartFadingInEvent.Listeners -= StartFadeIn;
            OnFadingInEndedEvent.Listeners -= OnFadeInEnded;
        }

        private void StartFadeIn(StartFadingInEvent info)
        {
            Entities.ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                cameraFade.OldFadingSpeedFactor = cameraFade.FadingSpeed;

                if (info.SpeedOverride != -1.0f)
                    cameraFade.FadingSpeed = info.SpeedOverride;

                EntityManager.AddComponentData(e, new CameraFadeIn());
            });
        }

        private void OnFadeInEnded(OnFadingInEndedEvent info)
        {
            Entities.ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                EntityManager.RemoveComponent<CameraFadeIn>(e);
                CameraFadeSystem.ResetParameters(ref cameraFade);
            });
        }
    }
}