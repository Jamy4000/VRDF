﻿using UnityEngine;

namespace VRSF.Core.Raycast
{
    public struct RaycastHitVariable
    {
        public RaycastHit Value;
        public bool IsNull;

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