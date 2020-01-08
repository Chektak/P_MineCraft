using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

//namespace UnityStandardAssets.Characters.FirstPerson
//{
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

    private UIManager uiMgr;
    private GameManager gameMgr;
    private void Start()
    {
        uiMgr = UIManager.Instance;
        gameMgr = GameManager.Instance;

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

        if (Input.GetKeyDown(KeyCode.E))
            uiMgr.panel_Inventory.SetActive(!uiMgr.panel_Inventory.activeSelf);

        if (Input.mouseScrollDelta.y > 0)
            gameMgr.inventory.ItemBar_SelectSlot(1);
        else if (Input.mouseScrollDelta.y < 0)
            gameMgr.inventory.ItemBar_SelectSlot(-1);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            gameMgr.inventory.ItemBar_SelectSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            gameMgr.inventory.ItemBar_SelectSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            gameMgr.inventory.ItemBar_SelectSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            gameMgr.inventory.ItemBar_SelectSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            gameMgr.inventory.ItemBar_SelectSlot(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            gameMgr.inventory.ItemBar_SelectSlot(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            gameMgr.inventory.ItemBar_SelectSlot(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            gameMgr.inventory.ItemBar_SelectSlot(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            gameMgr.inventory.ItemBar_SelectSlot(8);

        RaycastHit hit;

        //화면의 정가운데 위치에서 ray변수를 만든다.
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //블록 파괴
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 6f))
            {
                Vector3 blockPos = hit.transform.position;

                //맨 아래 블록은 소멸되지 않게 한다.
                if (blockPos.y <= 0) return;

                gameMgr.mapGenerator.worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;

                Particle par = Instantiate(gameMgr.paticles[0], blockPos, Quaternion.LookRotation(Vector3.up));
                par.ChangeMaterial(hit.collider.gameObject.GetComponent<MeshRenderer>().material);
                Destroy(hit.collider.gameObject);

                //자기 자신을 뺀 이웃들을 인스턴스화한다
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            //맵 밖 블록에는 접근하지 않도록 한다
                            if (blockPos.x + x < 0 || blockPos.x + x >= MapGenerator.width_x) continue;
                            if (blockPos.y + y < 0 || blockPos.y + y >= MapGenerator.height) continue;
                            if (blockPos.z + z < 0 || blockPos.z + z >= MapGenerator.width_z) continue;

                            Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                            gameMgr.mapGenerator.RenderBlock(neighbour);
                        }
                    }
                }
            }
        }

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red);

        if (Physics.Raycast(ray, out hit, 6f))
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red);
        }
        //블록 설치
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, 6f))
            {
                Vector3 point = ray.GetPoint(hit.distance);

                Vector3 axisSide = GetAxisSide(hit.transform.position - ray.GetPoint(hit.distance));
                Vector3 blockPos = hit.transform.position - axisSide;

                gameMgr.mapGenerator.CreateBlock(blockPos, true, Block.Type.Sand);
            }
        }
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
    }

    float getInput(out Vector2 input) {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        input = new Vector2(horizontal, vertical);
        if (input.sqrMagnitude > 1) input.Normalize();
#if !MOBILE_INPUT //대쉬 기능

        if (Input.GetKeyDown(KeyCode.W)) {
            StartCoroutine(addRunStack(0.2f));
        }
#endif//speed를 리턴한다.
        return (m_RunStack>=2) ? m_RunSpeed : m_WalkSpeed;
    }

    IEnumerator addRunStack(float duration) {
        while (duration > 0) {
            yield return null;
            if (Input.GetKeyDown(KeyCode.W))
                m_RunStack = 2;
            duration -= Time.deltaTime;
           
        }
        while (m_RunStack >= 2)
        {

            if (!(Input.GetKey(KeyCode.W)))
                m_RunStack = 0;
            yield return null;
        }
        yield break;
    }

    /// <summary>
    /// 사사오입 없는 반올림을 직접 구현함
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    float round(float value) {
        if (value - (int)value > 0.5)
            value = (int)value + 1;
        else
            value = (int)value;
        return value;
    }

    /// <summary>
    /// x,y,z축중 절댓값이 가장 큰 축에 대해 1이나 -1을 부여합니다.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    Vector3 GetAxisSide(Vector3 point) {
        float [] axiss = new float[3];
        axiss[0] = point.x;
        axiss[1] = point.y;
        axiss[2] = point.z;
        int largeNumIndex = 0;

        for (int i = 1; i < 3; i++) {
            //절댓값이 가장 큰 축을 찾는다
            if (Mathf.Abs(axiss[largeNumIndex]) < Mathf.Abs(axiss[i]))
                largeNumIndex = i;
        }

        switch (largeNumIndex) {
            //x축이 가장 절댓값이 크다면
            case 0:
                point.y = 0;
                point.z = 0;
                //x축이 양수라면
                if (axiss[largeNumIndex] > 0)
                    point.x = 1;
                else
                    point.x = -1;
                return point;
            //y축이 가장 절댓값이 크다면
            case 1:
                point.x = 0;
                point.z = 0;
                //y축이 양수라면
                if (axiss[largeNumIndex] > 0)
                    point.y = 1;
                else
                    point.y = -1;
                return point;
            //z축이 가장 절댓값이 크다면
            case 2:
                point.x = 0;
                point.y = 0;
                //z축이 양수라면
                if (axiss[largeNumIndex] > 0)
                    point.z = 1;
                else
                    point.z = -1;
                return point;
            default:
                Debug.Log("축 절댓값 인덱스가 잘못되었습니다");
                return Vector3.zero;
        }
    }

  
}


