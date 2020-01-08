using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("아이템 코드가 담겨지는 데이터상의 인벤토리")]
    [Header("Empty대신 -1을 사용한다")]
    public Dictionary<int, Item> inven = new Dictionary<int, Item>();

    private int ItemBar_nowSelectSlot {
        get {
            return itemBar_nowSelectSlot;
        }
        set {
            itemBar_nowSelectSlot=value;
            if ((object)inven[itemBar_nowSelectSlot] != null)
            {
                uiMgr.text_ItemBarItemName.text = inven[itemBar_nowSelectSlot].itemName;
            }
            else
                uiMgr.text_ItemBarItemName.text = "";
        }
    }
    private int itemBar_nowSelectSlot;
    private UIManager uiMgr;
    void Start()
    {
        uiMgr = UIManager.Instance;
        for (int i = 0; i < 36; i++)
        {
            //Empty대신 -1을 사용
            inven[i] = null;
        }

        uiMgr.img_ItemBarFrame[0].sprite = uiMgr.sprite_ItemBarSlotOn;
        ItemBar_nowSelectSlot = 0;
    }

    public void AddItem(int index, Item item) {
        if (index >= 36)
        {
            Debug.Log("인덱스가 36 이상입니다! AddItem실패!");
        }

        inven[index] = item;
        addRenderSprite(index, item.sprite);
        addRenderItemName(index, item.itemName);
        item.gameObject.SetActive(false);
    }
    

    void addRenderSprite(int index, Sprite spr) {
        if (index <= 8) {
            uiMgr.img_ItemBarPicture[index].sprite = spr;
        }
        //else 
    }

    void addRenderItemName(int index, string name) {
        if(ItemBar_nowSelectSlot==index)
        uiMgr.text_ItemBarItemName.text = name;
    }

    /// <summary>
    /// 매개변수가 0이라면 선택하고 있던 슬롯 기준 오른쪽, -1이라면 왼쪽, 1이상이라면 선택하고 있는 슬롯의 sprite를 바꾼다
    /// </summary>
    public void ItemBar_SelectSlot(int num)
    {
        //전에 셀렉하고 있던 슬롯의 sprite를 셀렉중이 아님을 표시하는 sprite로 바꾼다.
        uiMgr.img_ItemBarFrame[ItemBar_nowSelectSlot].sprite = uiMgr.sprite_ItemBarSlotOff;

        switch (num) {
            case -1:
                ItemBar_nowSelectSlot = itemBar_Clamp(ItemBar_nowSelectSlot + num);
                break;
            case 0:
                ItemBar_nowSelectSlot = itemBar_Clamp(ItemBar_nowSelectSlot + num + 1);
                break;
            default:
                ItemBar_nowSelectSlot = num;
                break;
        }

        //셀렉할 슬롯의 sprite를 셀렉중임을 표시하는 sprite로 바꾼다.
        uiMgr.img_ItemBarFrame[ItemBar_nowSelectSlot].sprite = uiMgr.sprite_ItemBarSlotOn;
    }

    /// <summary>
    /// 슬롯을 셀렉하는데 최솟값과 최댓값을 고정하게 해준다. 최댓값을 넘어갈 경우 최솟값으로 돌아간다.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int itemBar_Clamp(int value)
    {
        int min = 0;
        int max = uiMgr.img_ItemBarFrame.Length - 1;

        if (value < min)
            value = max;
        if (value > max)
            value = min;
        return value;
    }

    public Item GetItem() {

        return inven[ItemBar_nowSelectSlot];
    }
}
