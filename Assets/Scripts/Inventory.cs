using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public GameObject ItemPrefab;
    public Transform Content;
    private Dictionary<Component, InventoryItem> components = new Dictionary<Component, InventoryItem>();


    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.Instance.MainPlayer.OnItemAdd += AddItem;
        GameManager.Instance.MainPlayer.OnItemRemove += RemoveItem;
    }

    private void OnDisable()
    {
        GameManager.Instance.MainPlayer.OnItemAdd -= AddItem;
        GameManager.Instance.MainPlayer.OnItemRemove -= RemoveItem;
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
            item = go.GetComponent<InventoryItem>().OnAddedToInventory(component, qty);
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

        var component = Content.GetChild(position).GetComponent<InventoryItem>().AlchemicComponent;
        //RemoveItem(component, 0);
        GameManager.Instance.MainPlayer.UseElement(component);
    }

    public void TestAdd()
    {
        Component asset = ScriptableObject.CreateInstance<Component>();
        for (int i = 0, total = UnityEngine.Random.Range(1, 10); i < total; i++)
        {
            GameManager.Instance.MainPlayer.AddComponent(asset);
        }
    }

    public void TestRemove()
    {
        int idx = UnityEngine.Random.Range(0, Content.childCount - 1);
        Debug.Log($"Remove {idx}");
        RemoveAt(idx);
    }
}
