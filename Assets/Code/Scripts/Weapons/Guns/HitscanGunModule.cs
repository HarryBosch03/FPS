using Code.Scripts.Utility;
using Code.Scripts.Biped;
using UnityEngine;

namespace Code.Scripts.Weapons.Guns
{
    [RequireComponent(typeof(GunController))]
    public class HitscanGunModule : GunEffect
    {
        [Space]
        [SerializeField] protected float tracerTime;

        protected LineRenderer shootTracer;
        
        protected override void Awake()
        {
            base.Awake();
            
            shootTracer = transform.DeepFindCallback("Tracer", t => t.GetComponent<LineRenderer>());
        }

        protected void Update()
        {
            if (shootTracer && Time.time - Gun.LastShootTime > tracerTime)
            {
                shootTracer.enabled = false;
            }
        }

        public override void Execute()
        {
            Vector3 end;

            if (Biped.TryGetLookingAt(out var hit))
            {
                end = hit.point;
                Gun.ProcessHit(hit);
            }
            else
            {
                end = Biped.Head.position + Biped.Head.forward * 1000.0f;
            }

            if (!shootTracer) return;
            
            shootTracer.enabled = true;
            shootTracer.positionCount = 2;
            shootTracer.SetPosition(0, Gun.BlendWithViewport(Gun.ShootPoint.position, 1.0f));
            shootTracer.SetPosition(1, Gun.BlendWithViewport(end, 0.0f));
        }
    }
}
