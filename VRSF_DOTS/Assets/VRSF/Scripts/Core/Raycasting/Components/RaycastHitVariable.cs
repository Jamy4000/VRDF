using UnityEngine;

namespace VRSF.Core.Raycast
{
    public class RaycastHitVariable
    {
        public RaycastHit Value = new RaycastHit();
        public bool IsNull = true;

        public void SetIsNull(bool value)
        {
            IsNull = value;
        }

        public bool RaycastHitIsOnUI()
        {
            return Value.collider != null && Value.collider.gameObject.layer == LayerMask.NameToLayer("UI");
        }

        public bool RaycastHitIsNotOnUI()
        {
            return Value.collider == null || Value.collider.gameObject.layer != LayerMask.NameToLayer("UI");
        }
    }
}