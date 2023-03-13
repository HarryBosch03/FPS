using Code.Scripts.Weapons.Projectiles;
using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    public class ProjectileGunModule : GunEffect
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float speed, size = 1.0f, lifetime;
        [SerializeField] private bool gravity = true;
        
        public override void Execute()
        {
            projectilePrefab.Spawn(Gun.ShootPoint, Gun.Damage, speed, size, lifetime, gravity, Gun.IsPlayer);
        }
    }
}