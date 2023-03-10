using UnityEngine;

namespace Code.Scripts.Weapons.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        private const float SkinWidth = 0.1f;

        [SerializeField] private float radius;
        [SerializeField] private LayerMask collisionMask;
        
        private new Rigidbody rigidbody;

        private float speed, lifetime, scale;
        private float startTime;
        public ProjectileHitCallback callback;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            rigidbody.velocity = transform.forward * speed;
            rigidbody.useGravity = false;
            transform.localScale = Vector3.one * scale;

            startTime = Time.time;
        }

        private void FixedUpdate()
        {
            var currentSpeed = rigidbody.velocity.magnitude;
            var ray = new Ray(rigidbody.position, rigidbody.velocity);
            if (Physics.SphereCast(ray, radius * scale, out var hit, currentSpeed * Time.deltaTime + SkinWidth, collisionMask))
            {
                callback(this, hit);
                Destroy(gameObject);
            }

            if (Time.time - startTime > lifetime)
            {
                Destroy(gameObject);
            }
        }

        public Projectile Spawn(Transform muzzlePoint, ProjectileHitCallback callback, float speed, float lifetime, float scale = 1.0f, bool useGravity = true)
        {
            var instance = Instantiate(this, muzzlePoint.position, muzzlePoint.rotation);
            instance.speed = speed;
            instance.lifetime = lifetime;
            instance.scale = scale;
            instance.callback = callback;

            instance.rigidbody.useGravity = useGravity;

            return instance;
        }

        public delegate void ProjectileHitCallback(Projectile projectile, RaycastHit hit);
    }
}
