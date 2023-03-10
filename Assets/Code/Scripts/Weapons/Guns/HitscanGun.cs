using Code.Scripts.Utility;
using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    public class HitscanGun : Gun
    {
        [Space]
        [SerializeField] protected float tracerTime;

        protected LineRenderer shootTracer;

        protected override void Awake()
        {
            base.Awake();
            shootTracer = transform.DeepFindCallback("Tracer", t => t.GetComponent<LineRenderer>());
        }

        protected override void Update()
        {
            if (shootTracer && Time.time - lastShootTime > tracerTime)
            {
                shootTracer.enabled = false;
            }

            base.Update();
        }

        protected override void ShootAction()
        {
            Vector3 end;

            if (Biped.TryGetLookingAt(out var hit))
            {
                end = hit.point;
                ProcessHit(hit);
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
    }
}
