using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mixing : MonoBehaviour
{
    public static Mixing Instance;

    public GameObject ItemPrefab;
    public Transform Content;
    private Dictionary<Component, InventoryItem> components = new Dictionary<Component, InventoryItem>();
    private Dictionary<Component, int> qty = new Dictionary<Component, int>();
    private List<GameObject> placeholders = new List<GameObject>();

    private const int MAX_COMPONENTS = 6;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Add placeholders
        SpawnPlaceholders();
    }

    private void SpawnPlaceholders()
    {
        for (int i = components.Count + placeholders.Count; i < MAX_COMPONENTS; ++i)
        {
            var go = Instantiate(ItemPrefab, Content);
            go.GetComponent<InventoryItem>().OnAddedAsPlaceholder();
            placeholders.Add(go);
            go.transform.SetAsLastSibling();
        }
    }

    public bool CanAddComponent(Component component)
    {
        if (components.Count < MAX_COMPONENTS)
        {
            return true;
        }

        return components.ContainsKey(component);
    }

    public void AddComponent(Component component)
    {
        if (components.TryGetValue(component, out InventoryItem item))
        {
            qty[component] += 1;
            item.UpdateQuantity(qty[component]);
        }
        else
        {
            var go = Instantiate(ItemPrefab, Content);
            item = go.GetComponent<InventoryItem>().OnAddedToMixing(component, 1);
            components.Add(component, item);
            qty.Add(component, 1);

            if (placeholders.Count > 0)
            {
                Destroy(placeholders[0].gameObject);
                placeholders.RemoveAt(0);
            }

            foreach (var placeholder in placeholders)
            {
                placeholder.transform.SetAsLastSibling();
            }
        }
    }

    public void RemoveComponent(Component component)
    {
        if (!components.TryGetValue(component, out InventoryItem item))
        {
            return;
        }

        if (qty[component] == 1)
        {
            // Destroy item
            components.Remove(component);
            qty.Remove(component);
            Destroy(item.gameObject);

            // Add placeholder
            SpawnPlaceholders();
        }
        else
        {
            qty[component] -= 1;
            item.UpdateQuantity(qty[component]);
        }
    }

    public void Mix()
    {
        if (components.Count == 0)
        {
            return;
        }

        // Find the receipe that best matches
        // Consume ethir number of elements
        foreach (var pair in components)
        {
            Destroy(pair.Value.gameObject);
        }
        components.Clear();
        qty.Clear();

        // Add the result
        Component asset = ScriptableObject.CreateInstance<Component>();
        GameManager.Instance.MainPlayer.AddComponent(asset);

        // Respawn
        SpawnPlaceholders();
    }
}
