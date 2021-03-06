using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIReceiptItem : MonoBehaviour
{
    public Image Name;
    public InventoryItem Final;
    public InventoryItem[] Components;


    public void WithReceipt(ReceiptComponents receipt)
    {
        Name.sprite = receipt.Final.NameSprite;
        Name.SetNativeSize();
        Final.OnAddedAsReceipt(receipt.Final);
        Final.Receipt = receipt;
        
        for (int i = 0; i < Components.Length; ++i)
        {
            Components[i].gameObject.SetActive(i < receipt.Components.Count);
            if (i >= receipt.Components.Count)
            {
                continue;
            }

            Components[i].OnAddedAsReceipt(receipt.Components[i]);
            Components[i].Receipt = receipt;
        }
    }
}
