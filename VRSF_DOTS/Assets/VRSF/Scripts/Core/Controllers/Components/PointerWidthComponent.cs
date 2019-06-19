using UnityEngine;

namespace VRSF.Core.Controllers
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class PointerWidthComponent : MonoBehaviour
    {
        [HideInInspector] public float _BaseStartWidth;
        [HideInInspector] public float _BaseEndWidth;
    }
}