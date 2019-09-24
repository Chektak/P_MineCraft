using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("아이템 코드가 담겨지는 데이터상의 인벤토리")]
    [Header("Empty대신 -1을 사용한다")]
    public int[] inven = new int[36];

    [Header("아이템 코드와 일치하는 인덱스에 아이템프리팹할당")]
    public Item[] droppedItemPrefabList;

    private int ItemBar_nowSelectSlot {
        get {
            return itemBar_nowSelectSlot;
        }
        set {
            itemBar_nowSelectSlot=value;
            if (inven[itemBar_nowSelectSlot] != -1)
            {
                UIManager.Instance.text_ItemBarItemName.text = droppedItemPrefabList[inven[itemBar_nowSelectSlot]].itemName;
            }
            else
                UIManager.Instance.text_ItemBarItemName.text = "";
        }
    }
    private int itemBar_nowSelectSlot;
    
    void Start()
    {
       
        for (int i = 0; i < 36; i++)
        {
            //Empty대신 -1을 사용
            inven[i] = -1;
        }

        UIManager.Instance.img_ItemBarFrame[0].sprite = UIManager.Instance.sprite_ItemBarSlotOn;
        ItemBar_nowSelectSlot = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            UIManager.Instance.panel_Inventory.SetActive(!UIManager.Instance.panel_Inventory.activeSelf);
        if (Input.mouseScrollDelta.y > 0)
            ItemBar_SelectRightSlot();
        else if (Input.mouseScrollDelta.y < 0)
            ItemBar_SelectLeftSlot();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ItemBar_SelectNumSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ItemBar_SelectNumSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ItemBar_SelectNumSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ItemBar_SelectNumSlot(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            ItemBar_SelectNumSlot(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            ItemBar_SelectNumSlot(6);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            ItemBar_SelectNumSlot(7);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            ItemBar_SelectNumSlot(8);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            ItemBar_SelectNumSlot(9);
    }
    public void AddItem(int index, Item item) {
        if (index >= 36)
        {
            Debug.Log("인덱스가 36 이상입니다! AddItem실패!");
        }

        addInvenItemCode(index, item.itemCode);
        addRenderSprite(index, item.sprite);
        addRenderItemName(index, item.itemName);
        item.gameObject.SetActive(false);
    }
    void addInvenItemCode(int index, int itemCode) {
        if (inven[index] != -1)
        {
            Debug.Log("이미 값이 할당되어있습니다! AddItem실패!");
            return;
        }
        
        inven[index] = itemCode;
    }

    void addRenderSprite(int index, Sprite spr) {
        if (index <= 8) {
            UIManager.Instance.img_ItemBarPicture[index].sprite = spr;
        }
        //else 
    }

    void addRenderItemName(int index, string name) {
        if(ItemBar_nowSelectSlot==index)
        UIManager.Instance.text_ItemBarItemName.text = name;
    }

    /// <summary>
    /// 선택하고 있던 슬롯 기준 오른쪽의 슬롯을 선택하게 한다.
    /// </summary>
    public void ItemBar_SelectRightSlot()
    {
        //전에 셀렉하고 있던 슬롯의 sprite를 셀렉중이 아님을 표시하는 sprite로 바꾼다.
        UIManager.Instance.img_ItemBarFrame[ItemBar_nowSelectSlot].sprite = UIManager.Instance.sprite_ItemBarSlotOff;

        ItemBar_nowSelectSlot = itemBar_Clamp(ItemBar_nowSelectSlot + 1);

        //셀렉할 슬롯의 sprite를 셀렉중임을 표시하는 sprite로 바꾼다.
        UIManager.Instance.img_ItemBarFrame[ItemBar_nowSelectSlot].sprite = UIManager.Instance.sprite_ItemBarSlotOn;
    }

    /// <summary>
    /// 선택하고 있던 슬롯 기준 왼쪽의 슬롯을 선택하게 한다.
    /// </summary>
    public void ItemBar_SelectLeftSlot()
    {
        //전에 셀렉하고 있던 슬롯의 sprite를 셀렉중이 아님을 표시하는 sprite로 바꾼다.
        UIManager.Instance.img_ItemBarFrame[ItemBar_nowSelectSlot].sprite = UIManager.Instance.sprite_ItemBarSlotOff;

        ItemBar_nowSelectSlot = itemBar_Clamp(ItemBar_nowSelectSlot - 1);

        //셀렉할 슬롯의 sprite를 셀렉중임을 표시하는 sprite로 바꾼다.
        UIManager.Instance.img_ItemBarFrame[ItemBar_nowSelectSlot].sprite = UIManager.Instance.sprite_ItemBarSlotOn;
    }

    /// <summary>
    /// 숫자를 매개변수로 받아 해당 숫자의 슬롯을 선택하게 한다.
    /// </summary>
    /// <param name="num"></param>
    public void ItemBar_SelectNumSlot(int num)
    {
        //전에 셀렉하고 있던 슬롯의 sprite를 셀렉중이 아님을 표시하는 sprite로 바꾼다.
        UIManager.Instance.img_ItemBarFrame[ItemBar_nowSelectSlot].sprite = UIManager.Instance.sprite_ItemBarSlotOff;

        ItemBar_nowSelectSlot = num - 1;

        //셀렉할 슬롯의 sprite를 셀렉중임을 표시하는 sprite로 바꾼다.
        UIManager.Instance.img_ItemBarFrame[ItemBar_nowSelectSlot].sprite = UIManager.Instance.sprite_ItemBarSlotOn;
    }

    /// <summary>
    /// 슬롯을 셀렉하는데 최솟값과 최댓값을 고정하게 해준다. 최댓값을 넘어갈 경우 최솟값으로 돌아간다.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int itemBar_Clamp(int value)
    {
        int min = 0;
        int max = UIManager.Instance.img_ItemBarFrame.Length - 1;

        if (value < min)
            value = max;
        if (value > max)
            value = min;
        return value;
    }
}
