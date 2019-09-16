using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    //뷰의 상태(플레이어에서 ChangeView함수와 함께 사용)
    public enum ViewState {
        FIRSTVIEW, SECONDVIEW, THIRDVIEW, NONE
    }

    //카메라와의 거리
    [Header("카메라와의 거리 : 1인칭일때")]
    public float firstViewDist = 0.2f;
    [Header("카메라와의 거리 : 2인칭일때")]
    public float secondViewDist = -6f;
    [Header("카메라와의 거리 : 3인칭일때")]
    public float thirdViewDist = 6f;

    //카메라 회전 속도
    public float xSpeed = 220.0f;
    public float ySpeed = 100.0f;
    //마우스 감도
    public float mouseSensitivity = 0.015f;
    //카메라 초기 위치
    private float x = 0.0f;
    private float y = 0.0f;
    //y값 제한 (위 아래 제한)
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    //내부 변수들
    private float dist;
    public ViewState viewState = ViewState.FIRSTVIEW;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //커서 고정
        Vector3 angles = transform.eulerAngles;
        Debug.Log(angles);
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        //카메라 회전속도 계산
        x += Input.GetAxis("Mouse X") * xSpeed * mouseSensitivity;
        y -= Input.GetAxis("Mouse Y") * ySpeed * mouseSensitivity;
        //앵글값 정하기(y값 제한)
        y = clampAngle(y, yMinLimit, yMaxLimit);

        //뷰 상태에 따른 시점 변화
        switch (viewState)
        {
            case ViewState.FIRSTVIEW:
                dist = firstViewDist;
                break;
            case ViewState.SECONDVIEW:
                dist = secondViewDist;
                break;
            case ViewState.THIRDVIEW:
                dist = thirdViewDist;
                break;
            default:
                dist = firstViewDist;
                Debug.Log("카메라 viewState가 default입니다");
                break;
        }
        

        //카메라 위치 변화 계산
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0, 0.9f, -dist) 
            + target.position + new Vector3(0.0f, 0, 0.0f);

        transform.rotation = rotation;
        target.rotation = Quaternion.Euler(0, x, 0);
        transform.position = position;
        //[출처] [Unity5] 유니티에서 1인칭 3인칭 카메라 만들기|작성자 한지윤
    }



    //****************************************************************
    //이 밑으로 커스텀 함수. 외부에서 쓰이는 함수는 이름의 앞글자를 대문자로 해준다

    //앵글의 최소,최대 제한
    float clampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void ChangeViewState(ViewState viewStateValue=ViewState.NONE) {
        if (viewStateValue == ViewState.NONE) {
            //일인칭이라면 이인칭, 이인칭이라면 삼인칭, 삼인칭이라면 일인칭으로 바꿈
            switch (viewState)
            {
                case ViewState.FIRSTVIEW:
                    viewState = ViewState.THIRDVIEW;
                break;
                case ViewState.SECONDVIEW:
                    viewState = ViewState.FIRSTVIEW;
                    break;
                case ViewState.THIRDVIEW:
                    viewState = ViewState.SECONDVIEW;
                    break;
                default:
                    viewState = ViewState.FIRSTVIEW;
                    Debug.Log("ChangeViewState함수에서)카메라 내부의 viewState변수가 default입니다");
                    break;
            }
        }
        else//값이 들어왔다면 그 값으로 상태를 바꿈
            viewState = viewStateValue;
    }
}
