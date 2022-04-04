using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryIcon : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.Instance.IsInventoryOpen())
        {
            GameManager.Instance.CloseInventory();
        }
        else
        {
            GameManager.Instance.OpenInventory();
        }
    }
}
