using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public GameObject MixingTooltip;
    public GameObject InventoryTooltip;

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
        mixingActive = false;
        inventoryActive = true;
    }

    private void MixingOpened()
    {
        MixingTooltip.SetActive(true);
        InventoryTooltip.SetActive(false);
        mixingActive = true;
        inventoryActive = false;
    }

    public void Show(Component component, bool hideMouseActions = false)
    {
        Name.text = component.Name;
        Text.text = component.Description;
        gameObject.SetActive(true);

        if (hideMouseActions)
        {
            MixingTooltip.SetActive(false);
            InventoryTooltip.SetActive(false);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        MixingTooltip.SetActive(mixingActive);
        InventoryTooltip.SetActive(inventoryActive);
    }
}
