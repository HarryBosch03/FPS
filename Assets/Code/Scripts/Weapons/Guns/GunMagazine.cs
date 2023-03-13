using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    public abstract class GunMagazine : GunModule
    {
        [SerializeField] private int magazineSize;

        private int currentMagazine;

        public abstract bool CanFire();
        public abstract bool Reload();
    }
}