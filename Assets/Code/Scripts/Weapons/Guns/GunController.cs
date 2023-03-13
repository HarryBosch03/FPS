using Cinemachine;
using Code.Scripts.Biped;
using Code.Scripts.Health;
using Code.Scripts.Player;
using Code.Scripts.Player.Inventory;
using Code.Scripts.Utility;
using Code.Scripts.Signals;
using Code.Scripts.Weapons.Projectiles;
using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class GunController : MonoBehaviour, IWeapon
    {
        private const float ViewmodelFOV = 50.0f;

        [SerializeField] protected int damage;

        [Space] 
        [SerializeField] protected GameObject hitEffect;
        
        [Space] 
        [SerializeField] protected float kickAngle;
        [SerializeField] protected float kickMagnitude;
        [SerializeField] protected float kickRandomness;

        private CinemachineVirtualCamera virtualCamera;
        private Camera cam;

        private GunEffect effect;
        private GunMagazine magazine;
        private GunTrigger trigger;
        public int Damage => damage;
        public Animator Animator { get; private set; }
        public BipedController Biped { get; set; }
        public Transform ShootPoint { get; private set; }
        public bool Shoot { get; set; }
        public float LastShootTime { get; private set; }
        public bool IsPlayer { get; private set; }
        
        public const string SignalShoot = "shoot";

        protected virtual void Awake()
        {
            Biped = GetComponentInParent<BipedController>();
            Animator = GetComponentInChildren<Animator>();

            ShootPoint = transform.DeepFind("Shoot Point");

            var shoot = transform.Find("Shoot");
            effect = shoot.GetComponent<GunEffect>();
            magazine = shoot.GetComponent<GunMagazine>();
            trigger = shoot.GetComponent<GunTrigger>();

            IsPlayer = GetComponentInParent<PlayerAvatar>();

            virtualCamera = Biped.gameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            cam = Camera.main;
        }

        protected virtual void Update()
        {
            TryShoot();
        }

        private void TryShoot()
        {
            if (!trigger.Fire()) return;
            if (magazine) if (!magazine.CanFire()) return;
            
            effect.Execute();
            
            Animator.Play("Shoot0", 0, 0.0f);
            Signal.Call(SignalShoot, gameObject);
            KickCamera();
            LastShootTime = Time.time;
        }

        public Vector3 BlendWithViewport(Vector3 p1, float percent)
        {
            if (!virtualCamera && Biped is PlayerBipedController) return p1;

            var p2 = ToV4(p1);
            var vCamTransform = virtualCamera.transform;

            var camToClip = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane)
                .inverse;
            var clipToView = Matrix4x4.Perspective(ViewmodelFOV, cam.aspect, cam.nearClipPlane, cam.farClipPlane);

            p2 = vCamTransform.worldToLocalMatrix * p2;
            p2 = camToClip * ToV4(p2);
            p2 = clipToView * ToV4(p2);
            p2 = vCamTransform.localToWorldMatrix * p2;

            return Vector3.Lerp(p1, p2, percent);
        }

        public Vector4 ToV4(Vector3 v) => new Vector4(v.x, v.y, v.z, 1.0f);

        public void ProcessHit(Projectile p, RaycastHit hit) => ProcessHit(hit);

        public void ProcessHit(RaycastHit hit)
        {
            Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));

            var health = hit.transform.GetComponentInParent<IDamageable>();
            health?.Damage(new DamageArgs(gameObject, damage));
        }
        
        public void Equip()
        {
            enabled = true;
            gameObject.SetActive(true);
        }

        public void Holster()
        {
            Animator.Play("Unequip");
            enabled = false;
        }

        protected virtual void KickCamera()
        {
            if (Biped is not PlayerBipedController player) return;
            
            var a = (Random.value * 2.0f - 1.0f) * kickAngle * Mathf.Deg2Rad;
            var kick = new Vector2(Mathf.Sin(a), Mathf.Cos(a)) * (Mathf.Lerp(1.0f, Random.value, kickRandomness) * kickMagnitude);
            player.KickCamera(kick);
        }
    }
}