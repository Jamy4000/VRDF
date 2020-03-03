using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeSystem : ComponentSystem
    {
        private EntityManager _entityManager;
        private RenderMesh _renderMesh;

        protected override void OnCreate()
        {
            SetupVR.OnSetupVRReady.Listeners += SetupVrIsReady;
            StartFadingInEvent.Listeners += OnStartFadingIn;
            StartFadingOutEvent.Listeners += OnStartFadingOut;
            OnFadingInEndedEvent.Listeners += OnFadeInEnded;

            base.OnCreate();
            _entityManager = World.EntityManager;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            Entities.ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                _renderMesh = _entityManager.GetSharedComponentData<RenderMesh>(e);
                return;
            });
        }

        protected override void OnUpdate()
        {
            bool materialWasSet = false;

            Entities.WithAny(typeof(CameraFadeOut), typeof(CameraFadeIn)).ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                if (!materialWasSet)
                {
                    RenderMesh newRend = _renderMesh;
                    newRend.material = HandlePlaneAlpha(ref cameraFade, newRend, EntityManager.HasComponent<CameraFadeIn>(e));
                    _renderMesh = newRend;
                    materialWasSet = true;
                }

                _entityManager.SetSharedComponentData(e, _renderMesh);
            });
        }

        protected override void OnDestroy()
        {
            SetupVR.OnSetupVRReady.Listeners -= SetupVrIsReady;
            StartFadingInEvent.Listeners -= OnStartFadingIn;
            StartFadingOutEvent.Listeners -= OnStartFadingOut;
            OnFadingInEndedEvent.Listeners -= OnFadeInEnded;

            base.OnDestroy();
        }

        /// <summary>
        /// Change the alpha of the fading canvas and set the current teleporting state if the fade in/out is done
        /// </summary>
        private Material HandlePlaneAlpha(ref CameraFadeParameters cameraFade, RenderMesh renderMesh, bool isFadingIn)
        {
            Material newMat = renderMesh.material;
            Color color = newMat.color;

            // If we are currently Fading In
            if (isFadingIn)
            {
                color.a -= Time.DeltaTime * cameraFade.FadingSpeed;

                // If the fadingIn is finished
                if (color.a < 0)
                {
                    this.Enabled = false;
                    new OnFadingInEndedEvent();
                }
            }
            // If we are currently Fading Out
            else
            {
                color.a += Time.DeltaTime * cameraFade.FadingSpeed;

                // if the alpha is completely dark, we're done with the fade Out
                if (color.a > 1)
                {
                    this.Enabled = cameraFade.ShouldImmediatlyFadeIn;
                    new OnFadingOutEndedEvent();
                }
            }

            newMat.color = color;
            return newMat;
        }

        public static void ResetParameters(ref CameraFadeParameters cameraFade)
        {
            cameraFade.FadingSpeed = cameraFade.OldFadingSpeedFactor;
        }

        protected void SetupVrIsReady(SetupVR.OnSetupVRReady _)
        {
            bool fadingOnStart = false;
            Entities.WithAny(typeof(CameraFadeOut), typeof(CameraFadeIn)).ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                if (cameraFade.FadeInOnSceneLoaded)
                    fadingOnStart = true;
            });
            SetFadingMaterialAlpha(fadingOnStart ? 1.0f : 0.0f);
            this.Enabled = fadingOnStart;
        }

        protected void OnStartFadingIn(StartFadingInEvent _)
        {
            SetFadingMaterialAlpha(1.0f);
            this.Enabled = true;
        }

        protected void OnStartFadingOut(StartFadingOutEvent _)
        {
            SetFadingMaterialAlpha(0.0f);
            this.Enabled = true;
        }

        private void OnFadeInEnded(OnFadingInEndedEvent _)
        {
            this.Enabled = false;
        }

        private void SetFadingMaterialAlpha(float newAlpha)
        {
            Entities.WithAny(typeof(CameraFadeOut), typeof(CameraFadeIn)).ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                _renderMesh = _entityManager.GetSharedComponentData<RenderMesh>(e);
                RenderMesh newRend = _renderMesh;
                Material newMat = newRend.material;
                Color color = newMat.color;
                color.a = newAlpha;
                newMat.color = color;
                _renderMesh = newRend;
                _entityManager.SetSharedComponentData(e, _renderMesh);
            });
        }
    }
}