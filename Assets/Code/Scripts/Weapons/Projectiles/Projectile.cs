using System;
using Code.Scripts.FX;
using Code.Scripts.Utility;
using UnityEngine;

namespace Code.Scripts.Weapons.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        private const float SkinWidth = 0.1f;
        
        [SerializeField] private LayerMask collisionMask = ~0;
        [SerializeField] private float radius;
        
        [Space] 
        [SerializeField] private GameObject hitPrefab;
        [SerializeField] private BulletHoles bulletHoles;

        private GameObject visuals;
        
        private new Rigidbody rigidbody;
        private int damage;
        private float speed, lifetime, scale;
        private bool shotFromPlayer;
        private float startTime;

        private int framesAlive;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();

            visuals = transform.DeepFind("Visuals").gameObject;
        }

        private void Start()
        {
            rigidbody.velocity = transform.forward * speed;
            rigidbody.useGravity = false;
            transform.localScale = Vector3.one * scale;

            startTime = Time.time;

            if (shotFromPlayer) visuals.SetActive(false);
        }

        private void FixedUpdate()
        {
            var currentSpeed = rigidbody.velocity.magnitude;
            var ray = new Ray(rigidbody.position, rigidbody.velocity);
            if (Physics.SphereCast(ray, radius * scale, out var hit, currentSpeed * Time.deltaTime + SkinWidth, collisionMask))
            {
                if (hitPrefab) Instantiate(hitPrefab, hit.point, Quaternion.LookRotation(Vector3.Reflect(ray.direction, hit.normal)));
                if (bulletHoles) bulletHoles.Spawn(hit.point, hit.normal);
                
                Destroy(gameObject);
            }

            if (Time.time - startTime > lifetime) Destroy(gameObject);
            
            
            if (shotFromPlayer && framesAlive == 1)
            {
                visuals.SetActive(true);
            }
            framesAlive++;
        }

        public Projectile Spawn(Transform muzzlePoint, int damage, float speed, float lifetime, float scale = 1.0f, bool useGravity = true, bool shotFromPlayer = false)
        {
            var instance = Instantiate(this, muzzlePoint.position, muzzlePoint.rotation);
            instance.damage = damage;
            instance.speed = speed;
            instance.lifetime = lifetime;
            instance.scale = scale;
            instance.shotFromPlayer = shotFromPlayer;

            instance.rigidbody.useGravity = useGravity;

            return instance;
        }
    }
}