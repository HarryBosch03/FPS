using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Code.Scripts.Biped
{
    public class BipedController : MonoBehaviour
    {
        const float SkinWidth = 0.1f;

        [SerializeField] private float moveSpeed = 15.0f;
        [SerializeField] private float accelerationTime = 0.1f;
        [SerializeField][Range(0.0f, 1.0f)] private float airAccelerationPenalty = 0.2f;

        [Space]
        [SerializeField] private float jumpHeight = 4.0f;
        [SerializeField] private float jumpGravity = 3.0f;
        [SerializeField] private float fallingGravity = 3.0f;

        [Space]
        [SerializeField]
        private int maxDoubleJumps = 1;

        [Space]
        [SerializeField]
        private float groundTestDistance = 1.0f;
        [SerializeField] private float groundTestRadius = 0.4f;
    
        protected Vector3 position;
        protected Vector3 velocity;
        protected Vector3 frameAcceleration;
        private bool jumpLast;
        private int doubleJumpsLeft;

        private Collider[] colliders;

        public Vector3 MoveDirection { get; set; }
        public bool Jump { get; set; }
        public Vector2 LookRotation { get; set; }
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

        private void FixedUpdate()
        {
            frameAcceleration = Gravity;

            PerformChecks();

            Actions();

            PhysicsStuff();
            SetNextFrameFlags();
        }

        protected virtual void PerformChecks()
        {
            CheckForGround();
        }

        protected virtual void Actions ()
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
                var distance = result.point.y - position.y + SkinWidth;

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
            var acceleration = 1.0f / accelerationTime;
            if (!Grounded) acceleration *= airAccelerationPenalty;

            var target = transform.TransformDirection(MoveDirection) * moveSpeed;
            var diff = (target - velocity);

            diff.y = 0.0f;
            diff = Vector3.ClampMagnitude(diff, moveSpeed);

            var force = diff * acceleration;

            this.frameAcceleration += force;
            this.frameAcceleration += GroundVelocity;
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
            frameAcceleration += Vector3.up * jumpForce / Time.deltaTime;

            if (velocity.y < 0.0f) frameAcceleration += Vector3.up * -velocity.y / Time.deltaTime;
        }

        private void Depenetrate()
        {
            var others = GetBroadPhase();

            var hits = new List<Pair<Vector3, float>>();

            foreach (var self in colliders)
            {
                foreach (var other in others)
                {
                    if (other.transform.IsChildOf(transform)) continue;

                    if (Physics.ComputePenetration(self, self.transform.position, self.transform.rotation, other, other.transform.position, other.transform.rotation, out Vector3 direction, out float distance))
                    {
                        hits.Add(new Pair<Vector3, float>(direction, distance));
                    }
                }
            }

            foreach (var hit in hits)
            {
                position += hit.a * hit.b;

                var dot = Vector3.Dot(hit.a, velocity);
                if (dot < 0.0f) velocity -= hit.a * dot;
            }
        }

        private Collider[] GetBroadPhase()
        {
            var bounds = GetBounds();
            return Physics.OverlapBox(bounds.center, bounds.extents);
        }

        public Bounds GetBounds ()
        {
            var bounds = colliders[0].bounds;
            foreach (var self in colliders)
            {
                bounds.Encapsulate(self.bounds);
            }
            bounds.Expand(SkinWidth);
            return bounds;
        }

        private void Integrate()
        {
            position += velocity * Time.deltaTime;
            velocity += frameAcceleration * Time.deltaTime;
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

        public bool TryGetLookingAt (out RaycastHit hit)
        {
            var ray = new Ray(Head.position, Head.forward);
            return Physics.Raycast(ray, out hit);
        }
    }
}
