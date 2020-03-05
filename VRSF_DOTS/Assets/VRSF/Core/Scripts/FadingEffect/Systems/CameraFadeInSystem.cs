using Unity.Entities;

namespace VRSF.Core.FadingEffect
{
    /// <summary>
    /// Handle the callbacks linked to the Fade In event (Start and End)
    /// </summary>
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

        /// <summary>
        /// When a new FadeIn effect is called, we add a CameraFadeIn component to help the CameraFadeSystem know what it needs to do
        /// </summary>
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

        /// <summary>
        /// When the Fading In event is done, we remove the CameraFadeIn event and reset the fading effect parameters
        /// </summary>
        private void OnFadeInEnded(OnFadingInEndedEvent _)
        {
            Entities.ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                EntityManager.RemoveComponent<CameraFadeIn>(e);
                CameraFadeSystem.ResetParameters(ref cameraFade);
            });
        }
    }
}