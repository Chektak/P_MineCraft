using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image[] img_ItemBar;

    public Sprite sprite_ItemBarSlotOff;
    public Sprite sprite_ItemBarSlotOn;
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static UIManager instance;

    private int itemBar_nowSelectSlot;
    void Awake()
    {
        if (UIManager.Instance != null)
        {
            DestroyImmediate(this);
        }
        else
            instance = this;
    }

    void Start()
    {
        img_ItemBar[0].sprite = sprite_ItemBarSlotOn;
        itemBar_nowSelectSlot = 0;
    }

    /// <summary>
    /// 선택하고 있던 슬롯 기준 오른쪽의 슬롯을 선택하게 한다.
    /// </summary>
    public void ItemBar_SelectRightSlot() {
        //전에 셀렉하고 있던 슬롯의 sprite를 셀렉중이 아님을 표시하는 sprite로 바꾼다.
        img_ItemBar[itemBar_nowSelectSlot].sprite = sprite_ItemBarSlotOff;

        itemBar_nowSelectSlot = itemBar_Clamp(itemBar_nowSelectSlot + 1);

        //셀렉할 슬롯의 sprite를 셀렉중임을 표시하는 sprite로 바꾼다.
        img_ItemBar[itemBar_nowSelectSlot].sprite = sprite_ItemBarSlotOn;
    }

    /// <summary>
    /// 선택하고 있던 슬롯 기준 왼쪽의 슬롯을 선택하게 한다.
    /// </summary>
    public void ItemBar_SelectLeftSlot() {
        //전에 셀렉하고 있던 슬롯의 sprite를 셀렉중이 아님을 표시하는 sprite로 바꾼다.
        img_ItemBar[itemBar_nowSelectSlot].sprite = sprite_ItemBarSlotOff;

        itemBar_nowSelectSlot = itemBar_Clamp(itemBar_nowSelectSlot - 1);

        //셀렉할 슬롯의 sprite를 셀렉중임을 표시하는 sprite로 바꾼다.
        img_ItemBar[itemBar_nowSelectSlot].sprite = sprite_ItemBarSlotOn;
    }

    /// <summary>
    /// 숫자를 매개변수로 받아 해당 숫자의 슬롯을 선택하게 한다.
    /// </summary>
    /// <param name="num"></param>
    public void ItemBar_SelectNumSlot(int num) {
        //전에 셀렉하고 있던 슬롯의 sprite를 셀렉중이 아님을 표시하는 sprite로 바꾼다.
        img_ItemBar[itemBar_nowSelectSlot].sprite = sprite_ItemBarSlotOff;

        itemBar_nowSelectSlot = num-1;

        //셀렉할 슬롯의 sprite를 셀렉중임을 표시하는 sprite로 바꾼다.
        img_ItemBar[itemBar_nowSelectSlot].sprite = sprite_ItemBarSlotOn;
    }

    /// <summary>
    /// 슬롯을 셀렉하는데 최솟값과 최댓값을 고정하게 해준다. 최댓값을 넘어갈 경우 최솟값으로 돌아간다.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int itemBar_Clamp(int value) {
        int min = 0;
        int max=img_ItemBar.Length-1;

        if (value < min)
            value = max;
        if (value > max)
            value = min;
        return value;
    }
}
