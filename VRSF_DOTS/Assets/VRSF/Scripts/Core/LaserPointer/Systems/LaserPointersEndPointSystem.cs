using Unity.Entities;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// System to set the end point of the pointer (only for controllers)
    /// </summary>
    public class LaserPointersEndPointSystem : ComponentSystem
    {
        struct Filter
        {
            public LaserPointerEndPoint EndPointComponent;
            public UnityEngine.Transform EndPointTransform;
            public UnityEngine.MeshRenderer EndPointRenderer;
        }

        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            //foreach (var e in GetEntities<Filter>()) 
            //{
            //    if (e.EndPointComponent.HitVariable.IsNull)
            //    {
            //        e.EndPointRenderer.enabled = false;
            //    }
            //    else
            //    {
            //        e.EndPointTransform.position = e.EndPointComponent.HitVariable.Value.point;
            //        e.EndPointRenderer.enabled = true;
            //    }
            //}
        }
        #endregion ComponentSystem_Methods
    }
}
