using System.Collections.Generic;
using Code.Scripts.Utility;
using Code.Scripts.Weapons;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] private string defaultGun;

        private List<IWeapon> weapons;
        public IWeapon CurrentGun { get; private set; }

        public bool Shoot { get; set; }

        private void Awake()
        {
            weapons = new List<IWeapon>(GetComponentsInChildren<IWeapon>());
        }

        private void Start()
        {
            foreach (var weapon in weapons)
            {
                weapon.gameObject.SetActive(false);
            }

            EquipGun(defaultGun);
        }

        private void Update()
        {
            UpdateCurrentWeapon();
        }

        private void UpdateCurrentWeapon()
        {
            if (CurrentGun == null) return;

            CurrentGun.Shoot = Shoot;
        }

        private void EquipGun(string gunName)
        {
            UnequipGun();

            CurrentGun = weapons.Find(e => CompareNames(e.name, gunName));
            CurrentGun?.gameObject.SetActive(true);
        }

        private void UnequipGun()
        {
            CurrentGun?.gameObject.SetActive(false);
        }

        public bool CompareNames(string a, string b) => Util.SimplifyName(ref a) == Util.SimplifyName(ref b);
    }
}
