using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("아이템 코드가 담겨지는 데이터상의 인벤토리")]
    [Header("Empty대신 -1을 사용한다")]
    public int[] inven = new int[36];

    public GameObject[] droppedItemPrefabList;
    void Start()
    {
        for (int i = 0; i < 36; i++)
        {
            //Empty대신 -1을 사용
            inven[i] = -1;
        }

    }

    public void AddItem(int index, Item item) {
        if (index >= 36)
        {
            Debug.Log("인덱스가 36 이상입니다! AddItem실패!");
        }

        addInvenItemCode(index, item.itemCode);
        addRenderSprite(index, item.sprite);
        addRenderItemName(item.itemName);
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

    void addRenderItemName(string name) {
        UIManager.Instance.text_ItemBarItemName.text = name;
    }
}
