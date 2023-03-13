using UnityEngine;

namespace Code.Scripts.FX
{
    public class EffectBundle : MonoBehaviour
    {
        [SerializeField] private float duration;

        private void Awake()
        {
            Destroy(gameObject, duration);
        }
    }
}
