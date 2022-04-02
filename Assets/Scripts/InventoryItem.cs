using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public Component AlchemicComponent;
    public Image Sprite;
    public TMP_Text Quantity;


    public void UpdateQuantity(int qty)
    {
        Quantity.text = qty.ToString();
    }

    public InventoryItem UpdateAll(Component component, int qty)
    {
        AlchemicComponent = component;
        UpdateQuantity(qty);
        return this;
    }
}
