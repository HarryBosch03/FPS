using System.Collections.Generic;
using Cinemachine;
using Code.Scripts.Biped;
using Code.Scripts.Editor_Attributes;
using Code.Scripts.Player;
using Code.Scripts.Utility;
using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    public abstract class Gun : MonoBehaviour, IWeapon
    {
        private const float ViewmodelFOV = 50.0f;

        [SerializeField] protected int baseDamage;
        [SerializeField] protected float fireRate;
        [SerializeField] protected bool fullAuto;

        [Space] 
        [SerializeField] protected int fireAnimationCount;

        [Space] 
        [SerializeField] protected float kickAngle;
        [SerializeField] protected float kickMagnitude;
        [SerializeField] [Percent] protected float kickRandomness;
        
        [Space]
        [SerializeField] protected List<ParticleSystem> shootEffects;
        
        private Animator animator;
        protected Transform shootPoint;
        
        private int fireAnimationIndex;
        protected float lastShootTime;
        
        private bool shootLast;

        private CinemachineVirtualCamera virtualCamera;
        private Camera cam;

        protected BipedController Biped { get; set; }
        public bool Shoot { get; set; }
        public bool IsPlayer { get; private set; }

        private void Start()
        {
            cam = Camera.main;

            IsPlayer = GetComponentInParent<PlayerAvatar>();
        }

        protected virtual void Awake()
        {
            Biped = GetComponentInParent<BipedController>();
            animator = GetComponentInChildren<Animator>();

            transform.DeepFind("Muzzle");
            shootPoint = transform.DeepFind("Shoot Point");

            virtualCamera = Biped.GetComponentInChildren<CinemachineVirtualCamera>();

            shootEffects.RemoveAll(e => !e);
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
            KickCamera();

            animator.Play($"Shoot{fireAnimationIndex}", 0, 0.0f);
            if (fireAnimationCount > 0) fireAnimationIndex = (fireAnimationIndex + 1) % fireAnimationCount;

            shootEffects.ForEach(e => e.Play());

            lastShootTime = Time.time;
        }

        protected virtual void KickCamera()
        {
            if (Biped is not PlayerBipedController controller) return;
            
            var a = Random.value * kickAngle * Mathf.Deg2Rad;
            var kick = new Vector2(Mathf.Sin(a), Mathf.Cos(a)) * (Mathf.Lerp(1.0f, Random.value, kickRandomness) * kickMagnitude);
            controller.KickCamera(kick);
        }

        protected abstract void ShootAction();

        protected Vector3 BlendWithViewport(Vector3 p1, float percent)
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

        public static Vector4 ToV4(Vector3 v) => new(v.x, v.y, v.z, 1.0f);

        public void Equip()
        {
            enabled = true;
            gameObject.SetActive(true);
        }

        public void Holster()
        {
            animator.Play("Unequip");
            enabled = false;
        }
    }
}