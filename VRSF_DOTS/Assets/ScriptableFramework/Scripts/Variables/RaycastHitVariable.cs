using UnityEngine;

namespace ScriptableFramework.Variables
{
    [CreateAssetMenu(menuName = "Variables/RaycastHit")]
    public class RaycastHitVariable : VariableBase<RaycastHit>
    {
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