using System.Collections.Generic;
using Code.Scripts.Weapons;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] string defaultGun;

        List<IWeapon> weapons;
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
            if (CurrentGun != null)
            {
                CurrentGun.gameObject.SetActive(true);
            }
        }

        private void UnequipGun()
        {
            if (CurrentGun != null)
            {
                CurrentGun.gameObject.SetActive(false);
            }
        }

        public bool CompareNames(string a, string b) => SimplifyName(a) == SimplifyName(b);
        public string SimplifyName(string text) => string.IsNullOrEmpty(text) ? string.Empty : text.Trim().Replace(" ", "").ToLower();
    }
}
