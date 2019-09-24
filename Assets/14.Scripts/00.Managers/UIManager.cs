using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject panel_Inventory;
    public Text text_ItemBarItemName;
    
    public Image[] img_ItemBarPicture;
    public Image[] img_ItemBarFrame;
    public Image[] img_InvenPicture;

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

   

    
}
