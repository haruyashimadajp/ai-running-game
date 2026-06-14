using AIRunner.Commands;
using UnityEngine;

namespace AIRunner.Companion
{
    [RequireComponent(typeof(CharacterController))]
    public class AICompanionController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private float followDistance = 2.5f;
        [SerializeField] private float arriveThreshold = 0.5f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float goToDistance = 5f;

        private CharacterController controller;
        private Transform followTarget;
        private Vector3 moveDestination;
        private Vector3 velocity;
        private CompanionState state = CompanionState.Idle;

        public CompanionState CurrentState => state;
        public Transform FollowTarget => followTarget;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        public void Follow()
        {
            state = CompanionState.Follow;
        }

        public void Stop()
        {
            state = CompanionState.Idle;
        }

        public void Wait()
        {
            state = CompanionState.Waiting;
        }

        public void MoveTo(Vector3 destination)
        {
            moveDestination = destination;
            state = CompanionState.MoveTo;
        }

        public void MoveInDirection(Vector3 worldDirection)
        {
            MoveTo(transform.position + worldDirection.normalized * goToDistance);
        }

        public void Jump()
        {
            if (controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private void Update()
        {
            Vector3 horizontalMove = Vector3.zero;

            switch (state)
            {
                case CompanionState.Follow:
                    horizontalMove = GetFollowMove();
                    break;
                case CompanionState.MoveTo:
                    horizontalMove = GetMoveToMove();
                    break;
                case CompanionState.Idle:
                case CompanionState.Waiting:
                default:
                    break;
            }

            if (controller.isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }
            velocity.y += gravity * Time.deltaTime;

            Vector3 motion = new Vector3(horizontalMove.x, velocity.y, horizontalMove.z);
            controller.Move(motion * Time.deltaTime);
        }

        private Vector3 GetFollowMove()
        {
            if (followTarget == null)
            {
                return Vector3.zero;
            }

            Vector3 toTarget = followTarget.position - transform.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;

            if (distance <= followDistance)
            {
                return Vector3.zero;
            }

            Vector3 direction = toTarget.normalized;
            FaceDirection(direction);
            return direction * moveSpeed;
        }

        private Vector3 GetMoveToMove()
        {
            Vector3 toTarget = moveDestination - transform.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;

            if (distance <= arriveThreshold)
            {
                state = CompanionState.Idle;
                return Vector3.zero;
            }

            Vector3 direction = toTarget.normalized;
            FaceDirection(direction);
            return direction * moveSpeed;
        }

        private void FaceDirection(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }
    }
}
