using System.Collections.Generic;
using Cinemachine;
using Code.Scripts.Biped;
using Code.Scripts.Health;
using Code.Scripts.Player;
using Code.Scripts.Utility;
using Code.Scripts.Weapons.Projectiles;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Weapons.Guns
{
    public abstract class Gun : MonoBehaviour, IWeapon
    {
        private const float ViewmodelFOV = 50.0f;

        [FormerlySerializedAs("baseDamage")] [SerializeField] protected int damage;
        [SerializeField] protected float fireRate;
        [SerializeField] protected bool fullAuto;

        [Space]
        [SerializeField] protected int fireAnimationCount;

        [Space]
        [SerializeField] protected GameObject hitEffect;
        [SerializeField] protected List<ParticleSystem> shootEffects;

        private bool shootLast;
        protected float lastShootTime;
        private int fireAnimationIndex;
        protected bool lastAbility;

        protected Transform muzzle;
        protected Transform shootPoint;
        private Animator animator;

        private CinemachineVirtualCamera virtualCamera;

        public BipedController Biped { get; set; }
        public bool Shoot { get; set; }

        protected virtual void Awake()
        {
            Biped = GetComponentInParent<BipedController>();
            animator = GetComponentInChildren<Animator>();

            muzzle = transform.DeepFind("Muzzle");
            shootPoint = transform.DeepFind("Shoot Point");

            virtualCamera = Biped.gameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        }

        protected virtual void Update()
        {
            TryShoot();

            shootLast = Shoot;
        }

        private void TryShoot()
        {
            if (!Shoot) return;
            if (shootLast && !fullAuto) return;
            if (Time.time < lastShootTime + 60.0f / fireRate) return;

            ShootAction();

            animator.Play($"Shoot{fireAnimationIndex}", 0, 0.0f);
            if (fireAnimationCount > 0) fireAnimationIndex = (fireAnimationIndex + 1) % fireAnimationCount;

            shootEffects.ForEach(e => e.Play());

            lastShootTime = Time.time;
        }

        protected abstract void ShootAction();

        protected Vector3 BlendWithViewport(Vector3 p1, float percent)
        {
            if (!virtualCamera && Biped is PlayerBipedController) return p1;

            var p2 = ToV4(p1);
            var cam = Camera.main;
            var vCamTransform = virtualCamera.transform;

            var camToClip = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane).inverse;
            var clipToView = Matrix4x4.Perspective(ViewmodelFOV, cam.aspect, cam.nearClipPlane, cam.farClipPlane);

            p2 = vCamTransform.worldToLocalMatrix * p2;
            p2 = camToClip * ToV4(p2);
            p2 = clipToView * ToV4(p2);
            p2 = vCamTransform.localToWorldMatrix * p2;

            return Vector3.Lerp(p1, p2, percent);
        }

        public Vector4 ToV4(Vector3 v) => new Vector4(v.x, v.y, v.z, 1.0f);
        
        protected void ProcessHit(Projectile p, RaycastHit hit) => ProcessHit(hit);
        protected void ProcessHit(RaycastHit hit)
        {
            Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));

            var health = hit.transform.GetComponentInParent<IDamageable>();
            health?.Damage(new DamageArgs(gameObject, damage));
        }
    }
}
