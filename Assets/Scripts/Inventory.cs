using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Transform Content;
    private Dictionary<Component, InventoryItem> components = new Dictionary<Component, InventoryItem>();

    private void OnEnable()
    {
        
    }

    public void AddItem(Component component, int qty)
    {
        if (components.TryGetValue(component, out InventoryItem item)) 
        {
            item.UpdateQuantity(qty);
        }
        else
        {
            var go = Instantiate(ItemPrefab, Content);
            item = go.GetComponent<InventoryItem>().UpdateAll(component, qty);
            components.Add(component, item);
        }
    }

    public void RemoveItem(Component component, int qty)
    {
        if (!components.TryGetValue(component, out InventoryItem item))
        {
            return;
        }

        if (qty == 0)
        {
            components.Remove(component);
            Destroy(item.gameObject);
        }
        else
        {
            item.UpdateQuantity(qty);
        }
    }

    public void RemoveAt(int position)
    {
        if (position >= Content.childCount)
        {
            return;
        }

        RemoveItem(Content.GetChild(position).GetComponent<InventoryItem>().AlchemicComponent, 0);
    }

    public void TestAdd()
    {
        Component asset = ScriptableObject.CreateInstance<Component>();
        AddItem(asset, UnityEngine.Random.Range(1, 10));
    }

    public void TestRemove()
    {
        int idx = UnityEngine.Random.Range(0, Content.childCount - 1);
        Debug.Log($"Remove {idx}");
        RemoveAt(idx);
    }
}
