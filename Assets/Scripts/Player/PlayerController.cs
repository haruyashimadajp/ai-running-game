using UnityEngine;

namespace AIRunner.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float turnSpeed = 10f;

        private CharacterController controller;
        private Vector3 velocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(horizontal, 0f, vertical);

            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float speed = isRunning ? runSpeed : walkSpeed;

            Vector3 horizontalMove = input * speed;

            if (input.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(input, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            if (controller.isGrounded && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            Vector3 motion = new Vector3(horizontalMove.x, velocity.y, horizontalMove.z);
            controller.Move(motion * Time.deltaTime);
        }
    }
}
