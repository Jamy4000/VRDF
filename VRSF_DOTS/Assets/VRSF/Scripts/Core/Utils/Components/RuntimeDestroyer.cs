using UnityEngine;

namespace VRSF.Core.Utils
{
    public class RuntimeDestroyer : MonoBehaviour
    {
        [Header("If False, Destroy on Awake")]
        [SerializeField] private bool _destroyOnStart = true;

        private void Awake()
        {
            if (!_destroyOnStart)
                Destroy(gameObject);
        }

        private void Start()
        {
            Destroy(gameObject);
        }
    }
}