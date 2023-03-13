using Code.Scripts.FX;
using Code.Scripts.Utility;
using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    public class HitscanGun : Gun
    {
        [Space] 
        [SerializeField] protected GameObject hitEffect;

        [Space] 
        [SerializeField] protected float tracerTime;
        [SerializeField] protected BulletHoles bulletHoles;

        protected LineRenderer shootTracer;
        
        protected override void Awake()
        {
            base.Awake();
            shootTracer = transform.DeepFindCallback("Tracer", t => t.GetComponent<LineRenderer>());
        }

        protected override void Update()
        {
            if (shootTracer && Time.time - lastShootTime > tracerTime) shootTracer.enabled = false;

            base.Update();
        }

        protected override void ShootAction()
        {
            Vector3 end;

            if (Biped.TryGetLookingAt(out var ray, out var hit))
            {
                end = hit.point;
                ProcessHit(ray, hit);
            }
            else
            {
                end = Biped.Head.position + Biped.Head.forward * 1000.0f;
            }

            if (!shootTracer) return;
            
            shootTracer.enabled = true;
            shootTracer.positionCount = 2;
            shootTracer.SetPosition(0, BlendWithViewport(shootPoint.position, 1.0f));
            shootTracer.SetPosition(1, BlendWithViewport(end, 0.0f));
        }

        protected void ProcessHit(Ray ray, RaycastHit hit)
        {
            if (hitEffect) Instantiate(hitEffect, hit.point, Quaternion.LookRotation(Vector3.Reflect(ray.direction, hit.normal)));
            if (bulletHoles) bulletHoles.Spawn(hit.point, hit.normal);
        }
    }
}