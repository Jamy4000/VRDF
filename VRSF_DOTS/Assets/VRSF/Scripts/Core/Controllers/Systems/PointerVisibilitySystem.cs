using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Make the Pointer appear only when it's hitting something
    /// </summary>
    public class PointerVisibilitySystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableRaycastComponent RaycastComp;
            public PointerVisibilityComponents PointerVisibility;
            public PointerWidthComponent PointerWidth;
            public LineRenderer PointerRenderer;
        }


        #region ComponentSystem_Methods
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            foreach (var e in GetEntities<Filter>())
            {
                e.PointerWidth._BaseStartWidth = e.PointerRenderer.startWidth;
                e.PointerWidth._BaseEndWidth = e.PointerRenderer.endWidth;
            }
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                SetPointerVisibility(e);
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Set the alpha of the pointer depending on its state
        /// </summary>
        private void SetPointerVisibility(Filter e)
        {
            // If the Gaze is supposed to be off
            switch (e.PointerVisibility.PointerState)
            {
                case EPointerState.ON:
                    e.PointerRenderer.startWidth = e.PointerWidth._BaseStartWidth;
                    e.PointerRenderer.endWidth = e.PointerWidth._BaseEndWidth;
                    break;

                case EPointerState.DISAPPEARING:
                    e.PointerRenderer.startWidth -= (Time.deltaTime * e.PointerVisibility.DisappearanceSpeed) / 1000;
                    e.PointerRenderer.endWidth -= (Time.deltaTime * e.PointerVisibility.DisappearanceSpeed) / 1000;

                    if (e.PointerRenderer.startWidth == 0.0f && e.PointerRenderer.endWidth == 0.0f)
                        e.PointerVisibility.PointerState = EPointerState.OFF;
                    break;
            }
        }
        #endregion PRIVATE_METHODS
    }
}