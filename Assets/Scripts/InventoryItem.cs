using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class InventoryItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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
    
    private void ReloadSprite()
    {
        Sprite.sprite = AlchemicComponent.Sprite;
        Sprite.SetNativeSize();
    }

    public InventoryItem OnAddedToInventory(Component component, int qty)
    {
        InMixing = false;
        IsPlaceHolder = false;
        AlchemicComponent = component;
        UpdateQuantity(qty);
        ReloadSprite();

        return this;
    }

    public InventoryItem OnAddedToMixing(Component component, int qty)
    {
        InMixing = true;
        IsPlaceHolder = false;
        AlchemicComponent = component;
        UpdateQuantity(qty);
        ReloadSprite();

        return this;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsPlaceHolder)
        {
            return;
        }

        if (GameManager.Instance.IsMixingOpen())
        {
            // Remove from player and set to mixing
            if (!InMixing)
            {
                if (!Mixing.Instance.CanAddComponent(AlchemicComponent))
                {
                    return;
                }

                while (GameManager.Instance.MainPlayer.HasElement(AlchemicComponent))
                {
                    GameManager.Instance.MainPlayer.RemoveElement(AlchemicComponent);
                    Mixing.Instance.AddComponent(AlchemicComponent);

                    // Only once (left / middle), or more?
                    if (eventData.button != PointerEventData.InputButton.Right)
                    {
                        break;
                    }
                }

                if (!GameManager.Instance.MainPlayer.HasElement(AlchemicComponent))
                {
                    Inventory.Instance.Tooltip.Hide();
                }
            }
            else
            {
                while (Mixing.Instance.HasComponent(AlchemicComponent))
                {
                    Mixing.Instance.RemoveComponent(AlchemicComponent);
                    GameManager.Instance.MainPlayer.AddComponent(AlchemicComponent);

                    // Only once (left / middle), or more?
                    if (eventData.button != PointerEventData.InputButton.Right)
                    {
                        break;
                    }
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Use item outside mixing
            GameManager.Instance.MainPlayer.UseElement(AlchemicComponent);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InMixing)
        {
            return;
        }

        Inventory.Instance.Tooltip.Show(AlchemicComponent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InMixing)
        {
            return;
        }

        Inventory.Instance.Tooltip.Hide();
    }
}
