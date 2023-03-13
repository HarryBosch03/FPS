using System.Collections.Generic;
using Code.Scripts.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Player
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] private string[] equippedWeapons;

        private List<IWeapon> weapons;
        private string lastWeaponName;

        public IWeapon CurrentWeapon { get; private set; }
        public bool Shoot { get; set; }

        private void Awake()
        {
            weapons = new List<IWeapon>(GetComponentsInChildren<IWeapon>());
        }

        private void Start()
        {
            foreach (var weapon in weapons) weapon.gameObject.SetActive(false);

            EquipWeapon(0);
        }

        private void Update()
        {
            UpdateCurrentWeapon();
        }

        private void UpdateCurrentWeapon()
        {
            if (CurrentWeapon == null) return;

            CurrentWeapon.Shoot = Shoot;
        }

        public void SwitchToLastWeapon () => EquipWeapon(lastWeaponName);
        public void EquipWeapon(int i) 
        {
            if (i >= 0 && i < equippedWeapons.Length) EquipWeapon(equippedWeapons[i]);
        }
            
        public void EquipWeapon(string weaponName)
        {
            if (weaponName == CurrentWeapon?.name && CurrentWeapon?.enabled == true) return;
            
            lastWeaponName = CurrentWeapon?.name;
            
            CurrentWeapon?.gameObject.SetActive(false);
            CurrentWeapon = null;
            
            CurrentWeapon = Find(weaponName);
            CurrentWeapon?.Equip();
        }

        public void HolsterWeapon()
        {
            CurrentWeapon.Holster();
        }

        private static bool CompareNames(string a, string b)
        {
            return SimplifyName(a) == SimplifyName(b);
        }

        private static string SimplifyName(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.Trim().Replace(" ", "").ToLower();
        }
        
        private IWeapon Find(string weaponName)
        {
            foreach (var weapon in weapons)
            {
                if (CompareNames(weaponName, weapon.name))
                {
                    return weapon;
                }
            }

            return null;
        }
    }
}