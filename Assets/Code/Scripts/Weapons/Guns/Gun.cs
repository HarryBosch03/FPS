using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public abstract class Gun : MonoBehaviour, IWeapon
{
    const float viewmodelFOV = 50.0f;

    [SerializeField] protected int baseDamage;
    [SerializeField] protected float fireRate;
    [SerializeField] protected bool fullAuto;

    [Space]
    [SerializeField] protected int fireAnimationCount;

    [Space]
    [SerializeField] protected GameObject hitEffect;
    [SerializeField] protected List<ParticleSystem> shootEffects;

    bool shootLast;
    protected float lastShootTime;
    int fireAnimationIndex;
    protected bool lastAbility;

    protected Transform muzzle;
    protected Transform shootPoint;
    Animator animator;

    CinemachineVirtualCamera virtualCamera;

    public BipedController Biped { get; set; }
    public bool Shoot { get; set; }

    protected virtual void Awake()
    {
        Biped = GetComponentInParent<BipedController>();
        animator = GetComponentInChildren<Animator>();

        muzzle = transform.DeepFind("Muzzle");
        shootPoint = transform.DeepFind("Shoot Point");

        virtualCamera = Biped.GetComponentInChildren<CinemachineVirtualCamera>();
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
        var camsform = virtualCamera.transform;

        var camToClip = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane).inverse;
        var clipToView = Matrix4x4.Perspective(viewmodelFOV, cam.aspect, cam.nearClipPlane, cam.farClipPlane);

        p2 = camsform.worldToLocalMatrix * p2;
        p2 = camToClip * ToV4(p2);
        p2 = clipToView * ToV4(p2);
        p2 = camsform.localToWorldMatrix * p2;

        return Vector3.Lerp(p1, p2, percent);
    }

    public Vector4 ToV4(Vector3 v) => new Vector4(v.x, v.y, v.z, 1.0f);
}
