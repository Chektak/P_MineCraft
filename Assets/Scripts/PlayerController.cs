using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerController : MonoBehaviour
    {
        public MouseLook m_MouseLook;
        public float m_StickToGroundForce;
        public float m_GravityMultiplier;
        public float m_JumpSpeed;
        public float m_WalkSpeed;
        public float m_RunSpeed;
        
        private CollisionFlags m_CollisionFlags;
        private bool m_pressedJump;
        private CharacterController m_CharacterController;
        private Vector3 m_MoveDir;
        private bool m_PreviouslyGrounded;
        private bool m_Jumping;
        private bool m_IsWalking;
        private Camera m_Camera;

        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Jumping = false;
            m_Camera = Camera.main;
            m_MouseLook.Init(transform, m_Camera.transform);

        }

        private void Update()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
            //착지했을때
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            //계단을 내려갈 때
            if (!m_CharacterController.isGrounded && !m_Jumping
                && m_PreviouslyGrounded) {
                m_MoveDir.y = 0f;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
                m_pressedJump = true;
            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        private void FixedUpdate()
        {
            float speed;
            Vector2 input;
            speed = getInput(out input);
            Vector3 desiredMove = transform.forward * input.y +
                transform.right * input.x;

            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, 
                m_CharacterController.radius, Vector3.down, 
                out hitInfo, m_CharacterController.height / 2f, 
                Physics.AllLayers, QueryTriggerInteraction.Ignore);

            //땅에서부터 플레이어에 향한 방향벡터
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal.normalized);
            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_pressedJump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    m_pressedJump = false;
                    m_Jumping = true;
                }
            }
            else {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            m_MouseLook.UpdateCursorLock();
        }

        float getInput(out Vector2 input) {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            input = new Vector2(horizontal, vertical);
            if (input.sqrMagnitude > 1) input.Normalize();
#if !MOBILE_INPUT //대쉬 기능
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif//speed를 리턴한다.
            return m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        }
    }
}
