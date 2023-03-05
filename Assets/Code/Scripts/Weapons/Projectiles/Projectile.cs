using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    const float SkinWidth = 0.1f;

    [SerializeField] float radius;
    [SerializeField] LayerMask collisionMask;

    [Space]
    [SerializeField] GameObject hitPrefab;

    Rigidbody rigidbody;

    float damage, speed, lifetime, scale;
    float startTime;

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
        var speed = rigidbody.velocity.magnitude;
        var ray = new Ray(rigidbody.position, rigidbody.velocity);
        if (Physics.SphereCast(ray, radius * scale, out var hit, speed * Time.deltaTime + SkinWidth, collisionMask))
        {
            if (hitPrefab)
            {
                Instantiate(hitPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            Destroy(gameObject);
        }

        if (Time.time - startTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    public Projectile Spawn(Transform muzzlePoint, float damage, float speed, float lifetime, float scale = 1.0f, bool useGravity = true)
    {
        var instance = Instantiate(this, muzzlePoint.position, muzzlePoint.rotation);
        instance.damage = damage;
        instance.speed = speed;
        instance.lifetime = lifetime;
        instance.scale = scale;

        instance.rigidbody.useGravity = useGravity;

        return instance;
    }
}
