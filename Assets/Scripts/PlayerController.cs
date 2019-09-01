using System;
using System.Collections;
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
        public float m_RunCamFieldView;
        private CollisionFlags m_CollisionFlags;
        private bool m_pressedJump;
        private int m_RunStack;
        private CharacterController m_CharacterController;
        private Vector3 m_MoveDir;
        private bool m_PreviouslyGrounded;
        private bool m_Jumping;
        private Camera m_Camera;
        private float m_OryginCamFieldView;
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Jumping = false;
            m_RunStack = 0;
            m_Camera = Camera.main;
            m_MouseLook.Init(transform, m_Camera.transform);
            m_OryginCamFieldView = m_Camera.fieldOfView;
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
                && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            //점프키를 누르고 있는데 캐릭터가 땅에 있으면 점프키 입력 판정에 성공하게 한다.
            if (Input.GetKey(KeyCode.Space) && m_CharacterController.isGrounded)
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

            //계단을 내려갈때 계단 옆면에 붙게 하는 기능입니다.
            //RaycastHit hitInfo;
            //Physics.SphereCast(transform.position, 
            //    m_CharacterController.radius+0.1f, Vector3.down, 
            //    out hitInfo, m_CharacterController.height / 2f+0.1f, 
            //    Physics.AllLayers, QueryTriggerInteraction.Ignore);

            ////hitInfo의 법선 평면으로부터 플레이어가 움직이려는 방향벡터를 투영합니다.
            //desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal.normalized);
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
            if (m_RunStack >= 2)
                m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_OryginCamFieldView + m_RunCamFieldView, 0.3f);
            else if (m_RunStack <= 2)
                m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_OryginCamFieldView, 0.3f);
            m_MouseLook.UpdateCursorLock();
            Debug.Log(m_RunStack);
        }

        float getInput(out Vector2 input) {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            input = new Vector2(horizontal, vertical);
            if (input.sqrMagnitude > 1) input.Normalize();
#if !MOBILE_INPUT //대쉬 기능

            if (Input.GetKeyDown(KeyCode.W)) {
                StartCoroutine(AddRunStack(0.2f));
            }
#endif//speed를 리턴한다.
            return (m_RunStack>=2) ? m_RunSpeed : m_WalkSpeed;
        }

        IEnumerator AddRunStack(float duration) {
            m_RunStack++;
            while (duration > 0) {
                    duration -= Time.deltaTime;
                yield return null;
            }
            while (m_RunStack >= 2)
            {
                
                if (!(Input.GetKey(KeyCode.W)))
                    break;
                yield return null;
            }
            m_RunStack--;
            yield break;
        }

    }
}
