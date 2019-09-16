using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rigidbody가 있을 때에만 이 스크립트를 사용할 수 있다.
[RequireComponent(typeof(Rigidbody))]
public class Item_AbsorbPlayer : MonoBehaviour
{
    [Header("플레이어가 SphereTrigger에 닿았을때 최소속도")]
    public float absorbMinSpeed;
    [Header("플레이어가 SphereTrigger에 닿았을때 최대속도")]
    public float absorbMaxSpeed;

    private Rigidbody rgd;
    //플레이어와 가까워질수록 속도가 빨라지게 하기위해 사용


    private void Start()
    {
        rgd = GetComponent<Rigidbody>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 dir = other.transform.position - transform.position;
            dir.Normalize();
            float absorbSpeed = Mathf.Lerp(absorbMinSpeed, absorbMaxSpeed, 0.5f);
            rgd.velocity = dir * absorbSpeed;
        }
    }
}
