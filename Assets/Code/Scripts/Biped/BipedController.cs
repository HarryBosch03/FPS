using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Utility;
using UnityEngine;

namespace Code.Scripts.Biped
{
    public class BipedController : MonoBehaviour
    {
        const float SkinWidth = 0.1f;

        [SerializeField] float moveSpeed = 15.0f;
        [SerializeField] float accelerationTime = 0.1f;
        [SerializeField][Range(0.0f, 1.0f)] float airAccelerationPenalty = 0.2f;

        [Space]
        [SerializeField] float jumpHeight = 4.0f;
        [SerializeField] float jumpGravity = 3.0f;
        [SerializeField] float fallingGravity = 3.0f;

        [Space]
        [SerializeField] int maxDoubleJumps = 1;

        [Space]
        [SerializeField] float groundTestDistance = 1.0f;
        [SerializeField] float groundTestRadius = 0.4f;
    
        protected Vector3 position;
        protected Vector3 velocity;
        protected Vector3 acceleration;
        bool jumpLast;
        int doubleJumpsLeft;

        Collider[] colliders;

        public Vector3 MoveDirection { get; set; }
        public bool Jump { get; set; }
        public virtual Vector2 LookRotation { get; set; }
        public Transform Head { get; private set; }
        public virtual Vector3 Gravity => Physics.gravity * (velocity.y > 0.0f && Jump ? jumpGravity : fallingGravity);

        public bool Grounded { get; private set; }
        public Vector3 GroundVelocity { get; private set; }
        public Vector3 RelativeVelocity => velocity - GroundVelocity;

        private void Awake()
        {
            Head = transform.Find("Head");

            colliders = GetComponentsInChildren<Collider>();
        }

        protected void FixedUpdate()
        {
            acceleration = Gravity;

            PerformChecks();

            FixedUpdateActions();

            PhysicsStuff();
            SetNextFrameFlags();
        }

        protected virtual void PerformChecks()
        {
            CheckForGround();
        }

        protected virtual void FixedUpdateActions ()
        {
            Move();
            JumpAction();
        }

        protected virtual void PhysicsStuff()
        {
            Integrate();
            Depenetrate();

            transform.position = position;
        }

        protected virtual void SetNextFrameFlags()
        {
            jumpLast = Jump;
        }

        private void CheckForGround()
        {
            var ray = new Ray(position + Vector3.up * groundTestDistance, Vector3.down);
            var results = Physics.SphereCastAll(ray, groundTestRadius, groundTestDistance + SkinWidth).OrderBy(e => e.distance);

            Grounded = false;
            GroundVelocity = Vector3.zero;

            foreach (var result in results)
            {
                float distance = result.point.y - position.y + SkinWidth;

                if (result.transform.IsChildOf(transform)) continue;
                if (distance < 0.0f) continue;

                Grounded = true;
                doubleJumpsLeft = maxDoubleJumps;
                if (result.rigidbody)
                {
                    GroundVelocity = result.rigidbody.velocity;
                }

                position += Vector3.up * (distance - SkinWidth);
                if (velocity.y < 0.0f) velocity.y = 0.0f;

                break;
            }
        }

        private void Move()
        {
            float acceleration = 1.0f / accelerationTime;
            if (!Grounded) acceleration *= airAccelerationPenalty;

            Vector3 target = transform.TransformDirection(MoveDirection) * moveSpeed;
            Vector3 diff = (target - velocity);

            diff.y = 0.0f;
            diff = Vector3.ClampMagnitude(diff, moveSpeed);

            Vector3 force = diff * acceleration;

            this.acceleration += force;
            this.acceleration += GroundVelocity;
        }

        private void JumpAction()
        {
            if (!Jump) return;
            if (jumpLast) return;
        
            if (!Grounded)
            {
                if (doubleJumpsLeft > 0) doubleJumpsLeft--;
                else return;
            }

            var jumpForce = Mathf.Sqrt(2.0f * -Physics.gravity.y * jumpGravity * jumpHeight);
            acceleration += Vector3.up * jumpForce / Time.deltaTime;

            if (velocity.y < 0.0f) acceleration += Vector3.up * -velocity.y / Time.deltaTime;
        }

        private void Depenetrate()
        {
            var others = DoBroardPhase();

            List<Pair<Vector3, float>> hits = new List<Pair<Vector3, float>>();

            foreach (var collider in colliders)
            {
                foreach (var other in others)
                {
                    if (other.transform.IsChildOf(transform)) continue;

                    if (Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation, other, other.transform.position, other.transform.rotation, out Vector3 direction, out float distance))
                    {
                        hits.Add(new Pair<Vector3, float>(direction, distance));
                    }
                }
            }

            foreach (var hit in hits)
            {
                position += hit.a * hit.b;

                float dot = Vector3.Dot(hit.a, velocity);
                if (dot < 0.0f) velocity -= hit.a * dot;
            }
        }

        private Collider[] DoBroardPhase()
        {
            var bounds = GetBounds();
            return Physics.OverlapBox(bounds.center, bounds.extents);
        }

        public Bounds GetBounds ()
        {
            var bounds = colliders[0].bounds;
            foreach (var collider in colliders)
            {
                bounds.Encapsulate(collider.bounds);
            }
            bounds.Expand(SkinWidth);
            return bounds;
        }

        private void Integrate()
        {
            position += velocity * Time.deltaTime;
            velocity += acceleration * Time.deltaTime;
            transform.position = position;
        }

        private void Update()
        {
            RotateSelf();

            transform.position = position;
        }

        private void RotateSelf()
        {
            LookRotation = new Vector2(LookRotation.x, Mathf.Clamp(LookRotation.y, -90.0f, 90.0f));

            transform.rotation = Quaternion.Euler(0.0f, LookRotation.x, 0.0f);
            Head.rotation = Quaternion.Euler(-LookRotation.y, LookRotation.x, 0.0f);
        }

        public bool TryGetLookingAt(out RaycastHit hit) => TryGetLookingAt(out _, out hit);
        public bool TryGetLookingAt (out Ray ray, out RaycastHit hit)
        {
            ray = new Ray(Head.position, Head.forward);
            return Physics.Raycast(ray, out hit);
        }

        private void Reset()
        {
            if (!transform.Find("Head"))
            {
                var head = new GameObject("Head").transform;
                head.parent = transform;
                head.localPosition = Vector3.up * 1.8f;
            }
        }
    }
}
