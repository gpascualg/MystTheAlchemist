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

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Add placeholders
        for (int i = 0; i < 6; ++i)
        {
            var go = Instantiate(ItemPrefab, Content);
            go.GetComponent<InventoryItem>().OnAddedAsPlaceholder();
            placeholders.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
            var go = Instantiate(ItemPrefab, Content);
            go.GetComponent<InventoryItem>().OnAddedAsPlaceholder();
            placeholders.Add(go);
        }
        else
        {
            qty[component] -= 1;
            item.UpdateQuantity(qty[component]);
        }
    }
}
