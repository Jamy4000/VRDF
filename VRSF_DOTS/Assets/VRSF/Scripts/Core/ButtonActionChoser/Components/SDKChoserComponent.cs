using UnityEngine;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(BACGeneralComponent))]
    public class SDKChoserComponent : MonoBehaviour
    {
        [Header("SDKs using this ButtonActionChoserComponent")]
        [HideInInspector] public bool UseRift = true;
        [HideInInspector] public bool UseVive = true;
        [HideInInspector] public bool UseWMR = true;
        [HideInInspector] public bool UseSimulator = true;
    }
}