﻿using UnityEngine;

namespace VRDF.Core.Raycast
{
    public struct RaycastHitVariable
    {
        public RaycastHit Value;
        public bool IsNull;

        public RaycastHitVariable(bool isNullOnStart = true)
        {
            Value = new RaycastHit();
            IsNull = isNullOnStart;
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