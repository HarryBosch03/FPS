using System;
using Code.Scripts.Signals;
using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    public class ConfigurableGunTrigger : GunTrigger
    {
        [SerializeField] private bool singleFire;
        [SerializeField] private bool burst;
        [SerializeField] private bool auto;
        [SerializeField] int fireModeIndex;
        [SerializeField] float fireRate;

        [Space] [SerializeField] private int burstCount;

        private bool lastInput;
        private bool burstStarted;
        private int cBurst;

        private SignalListener listener;

        private void OnEnable()
        {
            listener = new SignalListener(GunController.SignalShoot, ShootCallback, gameObject);
        }

        private void OnDisable()
        {
            listener.Deregister();
        }

        public override bool Fire()
        {
            if (Time.time - Gun.LastShootTime < 60.0f / fireRate) return false;

            var res = fireModeIndex switch
            {
                0 => SingleFire(),
                1 => BurstFire(),
                2 => AutoFire(),
                _ => throw new System.IndexOutOfRangeException()
            };

            lastInput = Gun.Shoot;
            return res;
        }

        private bool SingleFire() => Gun.Shoot && !lastInput;

        private bool BurstFire()
        {
            if (burstStarted) return true;
            
            burstStarted = true;
            return SingleFire();
        }

        private bool AutoFire() => Gun.Shoot;

        public void ShootCallback()
        {
            if (!burstStarted) return;
            cBurst++;
            if (cBurst >= burstCount) burstStarted = false;
        }
    }
}