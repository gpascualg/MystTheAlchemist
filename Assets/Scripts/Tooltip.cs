using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public GameObject MixingTooltip;
    public GameObject InventoryTooltip;
    public GameObject ReceiptTooltip;

    public TMP_Text Name;
    public TMP_Text Text;

    private bool mixingActive;
    private bool inventoryActive;

    public void Subscribe()
    {
        GameManager.Instance.OnInventoryOpened += InventoryOpened;
        GameManager.Instance.OnMixingOpened += MixingOpened;
    }

    public void OnDestroy()
    {
        GameManager.Instance.OnInventoryOpened -= InventoryOpened;
        GameManager.Instance.OnMixingOpened -= MixingOpened;
    }

    private void InventoryOpened()
    {
        MixingTooltip.SetActive(false);
        InventoryTooltip.SetActive(true);
        ReceiptTooltip.SetActive(false);
        mixingActive = false;
        inventoryActive = true;
    }

    private void MixingOpened()
    {
        MixingTooltip.SetActive(true);
        InventoryTooltip.SetActive(false);
        ReceiptTooltip.SetActive(false);
        mixingActive = true;
        inventoryActive = false;
    }

    public void Show(Component component, bool hideMouseActions = false)
    {
        if (component.ComponentType == ComponentType.Potion && !GameManager.Instance.MainPlayer.Knows(component.ReceiptComponents))
        {
            Name.text = "Dubious Concoction";
            Text.text = "A yet to be drank concoction, one of a kind.";
        }
        else
        {
            Name.text = component.Name;
            Text.text = component.Description;
        }

        gameObject.SetActive(true);

        if (hideMouseActions)
        {
            MixingTooltip.SetActive(false);
            InventoryTooltip.SetActive(false);
            ReceiptTooltip.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        MixingTooltip.SetActive(mixingActive);
        InventoryTooltip.SetActive(inventoryActive);
        ReceiptTooltip.SetActive(false);
    }
}
