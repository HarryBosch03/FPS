using Code.Scripts.Biped;
using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    public class GunModule : MonoBehaviour
    {
        public GunController Gun { get; private set; }
        public BipedController Biped => Gun.Biped;

        protected virtual void Awake()
        {
            Gun = GetComponentInParent<GunController>();
        }
    }
}