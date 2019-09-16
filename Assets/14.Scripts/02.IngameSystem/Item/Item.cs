using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{
    [Header("아이템이 인벤토리상태일 때의 이미지")]
    public Sprite sprite;
    [Header("아이템 코드")]
    public int itemCode;
    public string itemName;

    public Item_Render item_Render;
    public Item_AbsorbPlayer item_AbsorbPlayer;
    public Item_AddInven item_AddInven;
    
}
