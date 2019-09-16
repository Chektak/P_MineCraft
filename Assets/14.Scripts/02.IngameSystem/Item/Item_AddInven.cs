using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_AddInven : MonoBehaviour
{
    private Item item;
    private void Start()
    {
        item = GetComponent<Item>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") {
            Debug.Log("Add!");
            Inventory inven = GameManager.Instance.player.GetComponent<Inventory>();
            inven.AddItem(0, item);
        }
    }

}
