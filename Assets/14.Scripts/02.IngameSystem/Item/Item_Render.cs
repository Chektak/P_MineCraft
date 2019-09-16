using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Render : MonoBehaviour
{
    public float rotSpeedY = 3f;
    public float waveSpeedY = 0.5f;
    public float waveMinY = -1f;
    public float waveMaxY = 1f;

    private bool isUp;
    private float updownSycleY;
    // Start is called before the first frame update
    void Start()
    {
        isUp = false;
        updownSycleY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp==false)
        {
            updownSycleY = Mathf.Lerp(updownSycleY, waveMaxY, waveSpeedY);
        }
        else
        {
            updownSycleY = Mathf.Lerp(updownSycleY, waveMinY, waveSpeedY);
        }
        if (updownSycleY >= waveMaxY)
            isUp = true;
        else if (updownSycleY <= waveMinY)
            isUp = false;
        transform.Rotate(new Vector3(0, rotSpeedY, 0));
        transform.localPosition = new Vector3(0, transform.localPosition.y+updownSycleY*Time.deltaTime, 0);
        
    }

}
