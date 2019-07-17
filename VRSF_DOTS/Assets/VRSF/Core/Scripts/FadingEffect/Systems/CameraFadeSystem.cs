using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Collections;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeSystem : ComponentSystem
    {
        private EntityManager _entityManager;
        private NativeArray<Entity> _entities;
        private RenderMesh _renderMesh;

        protected override void OnCreate()
        {
            StartFadingInEvent.Listeners += OnStartFadingIn;
            StartFadingOutEvent.Listeners += OnStartFadingOut;

            base.OnCreate();
            _entityManager = World.Active.EntityManager;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            int cameraFadeEntities = GetEntityQuery(typeof(CameraFadeParameters)).CalculateLength();
            _entities = new NativeArray<Entity>(cameraFadeEntities, Allocator.Persistent);
            Entities.ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                _renderMesh = _entityManager.GetSharedComponentData<RenderMesh>(e);
                return;
            });
        }

        protected override void OnUpdate()
        {
            int index = 0;
            bool materialWasSet = false;

            Entities.ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                _entities[index] = e;
                if (!materialWasSet && cameraFade.FadingInProgress)
                {
                    RenderMesh newRend = _renderMesh;
                    newRend.material = HandlePlaneAlpha(ref cameraFade, newRend);
                    _renderMesh = newRend;
                    materialWasSet = true;
                }
                index++;
            });

            for (int i = 0; i < _entities.Length; i++)
            {
                _entityManager.SetSharedComponentData(_entities[i], _renderMesh);
            }
        }

        protected override void OnDestroy()
        {
            StartFadingInEvent.Listeners -= OnStartFadingIn;
            StartFadingOutEvent.Listeners -= OnStartFadingOut;

            _entities.Dispose();
            base.OnDestroy();
        }

        /// <summary>
        /// Change the alpha of the fading canvas and set the current teleporting state if the fade in/out is done
        /// </summary>
        private Material HandlePlaneAlpha(ref CameraFadeParameters cameraFade, RenderMesh renderMesh)
        {
            Material newMat = renderMesh.material;
            Color color = newMat.color;

            // If we are currently Fading In
            if (cameraFade.IsFadingIn)
            {
                color.a -= Time.deltaTime * cameraFade.FadingSpeed;

                // If the fadingIn is finished
                if (color.a < 0)
                {
                    new OnFadingInEndedEvent();
                    this.Enabled = false;
                }
            }
            // If we are currently Fading Out
            else
            {
                color.a += Time.deltaTime * cameraFade.FadingSpeed;

                // if the alpha is completely dark, we're done with the fade Out
                if (color.a > 1)
                {
                    new OnFadingOutEndedEvent();
                    this.Enabled = !cameraFade.ShouldImmediatlyFadeIn;
                }
            }

            newMat.color = color;
            return newMat;
        }

        public static void ResetParameters(ref CameraFadeParameters cameraFade)
        {
            cameraFade.FadingInProgress = false;
            cameraFade.FadingSpeed = cameraFade.OldFadingSpeedFactor;
        }

        protected void OnStartFadingIn(StartFadingInEvent info)
        {
            this.Enabled = true;
        }

        protected void OnStartFadingOut(StartFadingOutEvent info)
        {
            this.Enabled = true;
        }
    }
}