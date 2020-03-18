using Unity.Entities;

namespace VRDF.Core.FadingEffect
{
    /// <summary>
    /// System used when we request a Fade In effect once SetupVR is done.
    /// </summary>
    public class CameraFadeOnSetupVRReadySystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += SetupVrIsReady;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (!VRDF_Components.SetupVRIsReady)
                return;

            // Calculate since how long the scene was loaded.
            Entities.ForEach((Entity e, ref CameraFadeOnStart cameraFadeOnStart) =>
            {
                cameraFadeOnStart.TimeSinceSceneLoaded += Time.DeltaTime;

                // if the timer is higher than the Time Before Fade In variable, we start the Fade In effect and deactivate this system
                if (cameraFadeOnStart.TimeSinceSceneLoaded > cameraFadeOnStart.TimeBeforeFadeIn)
                {
                    new StartFadingInEvent();
                    this.Enabled = false;
                }
            });
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= SetupVrIsReady;
            base.OnDestroy();
        }

        private void SetupVrIsReady(OnSetupVRReady _)
        {
            Entities.ForEach((Entity e, ref CameraFadeOnStart cameraFadeOnStart) =>
            {
                cameraFadeOnStart.TimeSinceSceneLoaded = 0.0f;
                this.Enabled = true;
            });
        }
    }
}