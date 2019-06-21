using UnityEngine;
using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeSystem : ComponentSystem
    {
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
            var componentArray = GetEntityQuery(typeof(UnityEngine.UI.Image), typeof(CameraFadeComponent)).ToComponentArray<CameraFadeComponent>();
            foreach (var cameraFade in componentArray)
            {
                if (cameraFade.FadingInProgress)
                    HandleImageAlpha(cameraFade);
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
            var componentArray = GetEntityQuery(typeof(UnityEngine.UI.Image), typeof(CameraFadeComponent)).ToComponentArray<CameraFadeComponent>();
            foreach (var cameraFade in componentArray)
            {
                cameraFade.FadingInProgress = true;
                cameraFade.IsFadingIn = true;
                cameraFade.OldFadingSpeedFactor = cameraFade.FadingSpeed;

                if (info.SpeedOverride != -1.0f)
                    cameraFade.FadingSpeed = info.SpeedOverride;
            }
        }

        private void StartFadeOut(StartFadingOutEvent info)
        {
            var componentArray = GetEntityQuery(typeof(UnityEngine.UI.Image), typeof(CameraFadeComponent)).ToComponentArray<CameraFadeComponent>();
            foreach (var cameraFade in componentArray)
            {
                cameraFade.FadingInProgress = true;
                cameraFade.IsFadingIn = false;
                cameraFade.ShouldImmediatlyFadeIn = info.ShouldFadeInWhenDone;
                cameraFade.OldFadingSpeedFactor = cameraFade.FadingSpeed;

                if (info.SpeedOverride != -1.0f)
                    cameraFade.FadingSpeed = info.SpeedOverride;
            }
        }

        private void OnFadeInEnded(OnFadingInEndedEvent info)
        {
            var componentArray = GetEntityQuery(typeof(UnityEngine.UI.Image), typeof(CameraFadeComponent)).ToComponentArray<CameraFadeComponent>();
            foreach (var cameraFade in componentArray)
            {
                ResetParameters(cameraFade);
            }
        }

        private void OnFadeOutEnded(OnFadingOutEndedEvent info)
        {
            var componentArray = GetEntityQuery(typeof(UnityEngine.UI.Image), typeof(CameraFadeComponent)).ToComponentArray<CameraFadeComponent>();
            foreach (var cameraFade in componentArray)
            {
                var overrideSpeed = cameraFade.FadingSpeed;

                ResetParameters(cameraFade);

                if (cameraFade.ShouldImmediatlyFadeIn)
                    new StartFadingInEvent(overrideSpeed);

                cameraFade.ShouldImmediatlyFadeIn = false;
            }
        }

        /// <summary>
        /// Change the alpha of the fading canvas and set the current teleporting state if the fade in/out is done
        /// </summary>
        private void HandleImageAlpha(CameraFadeComponent cameraFade)
        {
            var color = cameraFade.FadingImage.color;

            // If we are currently Fading In
            if (cameraFade.IsFadingIn)
            {
                color.a -= Time.deltaTime * cameraFade.FadingSpeed;

                // If the fadingIn is finished
                if (color.a < 0)
                    new OnFadingInEndedEvent();
            }
            // If we are currently Fading Out
            else
            {
                color.a += Time.deltaTime * cameraFade.FadingSpeed;

                // if the alpha is completely dark, we're done with the fade Out
                if (color.a > 1)
                    new OnFadingOutEndedEvent();
            }

            // We set the new alpha of the black image
            cameraFade.FadingImage.color = color;
        }
        
        private void StartFadingIn(OnSetupVRReady info)
        {
            Debug.LogError("CANNOT DO THAT as CameraFadeComponent is not a ComponentData.");
            var componentArray = GetEntityQuery(typeof(CameraFadeComponent)).ToComponentArray<CameraFadeComponent>();
            Debug.Log("componentArray " + componentArray.Length);
            foreach (var cameraFade in componentArray)
            {
                Debug.Log("cameraFade " + cameraFade.transform.name);
                cameraFade.FadingImage = cameraFade.GetComponent<UnityEngine.UI.Image>();
                var newColor = cameraFade.FadingImage.color;
                newColor.a = 1;
                cameraFade.FadingImage.color = newColor;
            }

            new StartFadingInEvent(0.5f);
        }

        private void ResetParameters(CameraFadeComponent fadeComponent)
        {
            fadeComponent.FadingInProgress = false;
            fadeComponent.FadingSpeed = fadeComponent.OldFadingSpeedFactor;
        }
    }
}