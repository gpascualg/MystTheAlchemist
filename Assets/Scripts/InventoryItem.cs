using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public Component AlchemicComponent;
    public Image Sprite;
    public TMP_Text Quantity;
    public bool InMixing;
    public bool IsPlaceHolder;

    public void UpdateQuantity(int qty)
    {
        Quantity.gameObject.SetActive(true);
        Quantity.text = qty.ToString();
    }

    public InventoryItem OnAddedAsPlaceholder()
    {
        Quantity.gameObject.SetActive(false);
        IsPlaceHolder = true;
        InMixing = true;

        return this;
    }

    public InventoryItem OnAddedToInventory(Component component, int qty)
    {
        InMixing = false;
        IsPlaceHolder = false;
        AlchemicComponent = component;
        UpdateQuantity(qty);

        return this;
    }

    public InventoryItem OnAddedToMixing(Component component, int qty)
    {
        InMixing = true;
        IsPlaceHolder = false;
        AlchemicComponent = component;
        UpdateQuantity(qty);

        return this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsPlaceHolder)
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!GameManager.Instance.IsMixingOpen())
            {
                return;
            }

            // Remove from player and set to mixing
            if (!InMixing)
            {
                if (!Mixing.Instance.CanAddComponent(AlchemicComponent))
                {
                    return;
                }

                GameManager.Instance.MainPlayer.UseElement(AlchemicComponent);
                Mixing.Instance.AddComponent(AlchemicComponent);
            }
            else
            {
                Mixing.Instance.RemoveComponent(AlchemicComponent);
                GameManager.Instance.MainPlayer.AddComponent(AlchemicComponent);
            }
        }

        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

}
