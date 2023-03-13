using Code.Scripts.Biped;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Player
{
    public class PlayerBipedController : BipedController
    {
        [Space] 
        [SerializeField] private float dashDistance;
        [SerializeField] private float dashDuration;
        
        [Space] 
        [SerializeField] private int maxDashCharges;
        [SerializeField] private float dashRechargeDelay;
        [SerializeField] private float dashRechargeTime;
        [FormerlySerializedAs("velocityCancelation")] [SerializeField] [Range(0.0f, 1.0f)] private float dashVelocityCancellation;

        [Space] 
        [SerializeField] private float kickDamper;

        private Vector3 dashDirection;
        private float lastDashTime;
        private bool dashing;
        private bool dashLast;
        private float dashTime = float.NegativeInfinity;

        private Vector2 kickPosition;
        private Vector2 kickVelocity;
        private Vector2 kickAcceleration;

        public bool Dash { get; set; }
        public float DashCharge { get; private set; }

        public int MaxDashCharges => maxDashCharges;

        public override Vector2 LookRotation
        {
            get => base.LookRotation + kickPosition;
            set => base.LookRotation = value - kickPosition;
        }

        protected override void FixedUpdateActions()
        {
            base.FixedUpdateActions();

            DashAction();

            kickAcceleration += -kickVelocity * kickDamper;

            kickPosition += kickVelocity * Time.deltaTime;
            kickVelocity += kickAcceleration * Time.deltaTime;

            kickAcceleration = Vector2.zero;
        }

        private void DashAction()
        {
            if (Grounded && Time.time - lastDashTime > dashRechargeDelay)
                DashCharge += Time.deltaTime / dashRechargeTime;
            DashCharge = Mathf.Clamp(DashCharge, 0.0f, maxDashCharges);

            if (dashing)
            {
                if (dashTime <= dashDuration)
                {
                    velocity = dashDirection * dashDistance / dashTime;
                    frameAcceleration = Vector3.zero;
                }
                else
                {
                    velocity = dashDirection * ((dashDistance / dashTime) * dashVelocityCancellation);
                    dashing = false;
                }
            }

            if (!Dash) return;
            if (dashLast) return;
            if (DashCharge <= 0.95f) return;

            if (MoveDirection.sqrMagnitude < 0.1f * 0.1f) return;

            dashDirection = transform.TransformDirection(MoveDirection).normalized;
            dashing = true;
            dashTime = 0.0f;
            DashCharge--;
            lastDashTime = Time.time;
        }

        protected override void SetNextFrameFlags()
        {
            base.SetNextFrameFlags();

            dashLast = Dash;
            dashTime += Time.deltaTime;
        }

        public void KickCamera(Vector2 kick)
        {
            kickAcceleration += kick;
        }
    }
}