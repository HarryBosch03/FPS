using Code.Scripts.Weapons.Projectiles;
using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    public class ProjectileGun : Gun
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float speed, size = 1.0f, lifetime;
        [SerializeField] private bool gravity = true;
        
        protected override void ShootAction()
        {
            projectilePrefab.Spawn(shootPoint, baseDamage, speed, size, lifetime, gravity, IsPlayer);
        }
    }
}