using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed=5.0f;
    public float jumpPower = 5;
    private Rigidbody rgd;
    public bool isFalling = false;
    void Start()
    {
        rgd=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 newPos=new Vector3();
        float newSpeed = speed;
        if (h > 0.1f)
        {
            newPos.x = 1;
        }
        if (h < -0.1f)
        {
                newPos.x = -1;
        }
        if (v > 0.1f)
        {
            newPos.z = 1;
        }
        if (v < -0.1f)
        {
            newPos.z = -1;
        }
        if (Input.GetKeyDown(KeyCode.Space) &&isFalling==false) {
            rgd.velocity+=new Vector3(0, jumpPower, 0);
            isFalling = true;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            newSpeed *= 2;
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            Camera.main.GetComponent<CameraFollow>().ChangeViewState();
        }
        transform.Translate(newPos*newSpeed * Time.deltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") {
            isFalling =false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") {
            isFalling = true;
        }
    }
}
