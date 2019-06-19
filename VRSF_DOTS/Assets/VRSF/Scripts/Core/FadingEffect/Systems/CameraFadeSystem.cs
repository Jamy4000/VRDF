using UnityEngine;
using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeSystem : ComponentSystem
    {
        private struct Filter
        {
            public UnityEngine.UI.Image FadingImage;
            public CameraFadeComponent CameraFade;
        }

        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += StartFadingIn;

            StartFadingInEvent.Listeners += StartFadeIn;
            StartFadingOutEvent.Listeners += StartFadeOut;

            OnFadingInEndedEvent.Listeners += OnFadeInEnded;
            OnFadingOutEndedEvent.Listeners += OnFadeOutEnded;

            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.CameraFade.FadingInProgress)
                {
                    HandleImageAlpha(e);
                }
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnCreateManager();
            OnSetupVRReady.Listeners -= StartFadingIn;

            StartFadingInEvent.Listeners -= StartFadeIn;
            StartFadingOutEvent.Listeners -= StartFadeOut;

            OnFadingInEndedEvent.Listeners -= OnFadeInEnded;
            OnFadingOutEndedEvent.Listeners -= OnFadeOutEnded;
        }

        private void StartFadeIn(StartFadingInEvent info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                e.CameraFade.FadingInProgress = true;
                e.CameraFade._IsFadingIn = true;
                e.CameraFade._OldFadingSpeedFactor = e.CameraFade.FadingSpeed;

                if (info.SpeedOverride != -1.0f)
                    e.CameraFade.FadingSpeed = info.SpeedOverride;
            }
        }

        private void StartFadeOut(StartFadingOutEvent info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                e.CameraFade.FadingInProgress = true;
                e.CameraFade._IsFadingIn = false;
                e.CameraFade._ShouldImmediatlyFadeIn = info.ShouldFadeInWhenDone;
                e.CameraFade._OldFadingSpeedFactor = e.CameraFade.FadingSpeed;

                if (info.SpeedOverride != -1.0f)
                    e.CameraFade.FadingSpeed = info.SpeedOverride;
            }
        }

        private void OnFadeInEnded(OnFadingInEndedEvent info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                ResetParameters(e.CameraFade);
            }
        }

        private void OnFadeOutEnded(OnFadingOutEndedEvent info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                var overrideSpeed = e.CameraFade.FadingSpeed;

                ResetParameters(e.CameraFade);

                if (e.CameraFade._ShouldImmediatlyFadeIn)
                    new StartFadingInEvent(overrideSpeed);

                e.CameraFade._ShouldImmediatlyFadeIn = false;
            }
        }

        /// <summary>
        /// Change the alpha of the fading canvas and set the current teleporting state if the fade in/out is done
        /// </summary>
        private void HandleImageAlpha(Filter e)
        {
            var color = e.FadingImage.color;

            // If we are currently Fading In
            if (e.CameraFade._IsFadingIn)
            {
                color.a -= Time.deltaTime * e.CameraFade.FadingSpeed;

                // If the fadingIn is finished
                if (color.a < 0)
                    new OnFadingInEndedEvent();
            }
            // If we are currently Fading Out
            else
            {
                color.a += Time.deltaTime * e.CameraFade.FadingSpeed;

                // if the alpha is completely dark, we're done with the fade Out
                if (color.a > 1)
                    new OnFadingOutEndedEvent();
            }

            // We set the new alpha of the black image
            e.FadingImage.color = color;
        }
        
        private void StartFadingIn(OnSetupVRReady info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                var newColor = e.FadingImage.color;
                newColor.a = 1;
                e.FadingImage.color = newColor;
            }

            new StartFadingInEvent(0.5f);
        }

        private void ResetParameters(CameraFadeComponent fadeComponent)
        {
            fadeComponent.FadingInProgress = false;
            fadeComponent.FadingSpeed = fadeComponent._OldFadingSpeedFactor;
        }
    }
}