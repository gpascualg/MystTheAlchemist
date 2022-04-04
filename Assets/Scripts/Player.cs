using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[System.Serializable]
public class JSONItem
{
    public JSONComponent Component;
    public int Quantity;
}

public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
{
    #region IComparer<TKey> Members

    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1; // Handle equality as being greater. Note: this will break Remove(key) or
        else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
            return result;
    }

    #endregion
}

public class Player : MonoBehaviour
{
    private SortedList<float, WorldComponent> nearCandidates = new SortedList<float, WorldComponent>(new DuplicateKeyComparer<float>());
    public SerializedDictionary<Component, int> Inventory = new SerializedDictionary<Component, int>();
    public Action<int> OnItemCollected;
    public Action<Component, int> OnItemAdd;
    public Action<Component, int> OnItemRemove;

    private float nearestComponentDist = float.PositiveInfinity;
    private WorldComponent nearestComponent;

    public List<ReceiptComponents> LearnedReceipts = new List<ReceiptComponents>();
    private HashSet<string> alreadyKnownReceipts = new HashSet<string>();
    public Action<ReceiptComponents> OnLearnedReceipt;

    public GameObject FloatingText;

    private Rigidbody2D rg;

    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();

        LearnedReceipts.Add(Receipts.Instance.FindReceiptAt(0));
        alreadyKnownReceipts.Add(Receipts.Instance.FindReceiptAt(0).GUID);
        OnLearnedReceipt?.Invoke(Receipts.Instance.FindReceiptAt(0));
    }

    private float DistanceTo(WorldComponent comp)
    {
        return (comp.transform.position - transform.position).sqrMagnitude;
    }

    public List<JSONItem> SerializeInventory()
    {
        List<JSONItem> result = new List<JSONItem>();
        foreach (var pair in Inventory)
        {
            result.Add(new JSONItem()
            {
                Component = pair.Key.Serialize(),
                Quantity = pair.Value
            });
        }
        return result;
    }

    public bool Knows(ReceiptComponents receipt)
    {
        return alreadyKnownReceipts.Contains(receipt.GUID);
    }

    public void AddToNearest(WorldComponent component)
    {
        float distance = DistanceTo(component);
        nearCandidates.Add(distance, component);

        if (distance < nearestComponentDist)
        {
            NormalNearest();
            nearestComponentDist = distance;
            nearestComponent = component;
            HighlightNearest();
        }
    }

    public void RemoveFromNearest(WorldComponent component)
    {
        for (int i = 0; i < nearCandidates.Count; i++)
        {
            if (nearCandidates.Values[i] == component)
            {
                nearCandidates.RemoveAt(i);
                break;
            }
        }

        if (component == nearestComponent)
        {
            NormalNearest();
            nearestComponent = null;
        }

        if (nearCandidates.Count > 0)
        {
            nearestComponentDist = nearCandidates.Keys[0];
            nearestComponent = nearCandidates.Values[0];
            HighlightNearest();
        }
    }

    private void NormalNearest()
    {
        if (nearestComponent != null)
        {
            nearestComponent.GetComponent<SpriteRenderer>().material = GameManager.Instance.NormalMaterial;
            GameManager.Instance.HideETooltip();
        }
    }

    private void HighlightNearest()
    {
        nearestComponent.GetComponent<SpriteRenderer>().material = GameManager.Instance.OutlineMaterial;
        GameManager.Instance.ShowETooltip(nearestComponent.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nearCandidates.Count > 0 && nearestComponent != null)
            {
                OnItemCollected?.Invoke(nearestComponent.WorldId);
                nearestComponent.Gather(this);
            }
        }

        if (nearCandidates.Count > 0 && rg.velocity.sqrMagnitude > 0)
        {
            SortedList<float, WorldComponent> resort = new SortedList<float, WorldComponent>(nearCandidates.Count, new DuplicateKeyComparer<float>());
            foreach (var comp in nearCandidates.Values)
            {
                resort.Add(DistanceTo(comp), comp);
            }
            nearCandidates = resort;

            if (nearCandidates.Values[0] != nearestComponent)
            {
                NormalNearest();
                nearestComponentDist = nearCandidates.Keys[0];
                nearestComponent = nearCandidates.Values[0];
                HighlightNearest();
            }
        }
    }

    public void AddComponent(Component AlchemicComponent)
    {
        if (Inventory.ContainsKey(AlchemicComponent))
        {
            Inventory[AlchemicComponent] += 1;
        }
        else
        {
            Inventory.Add(AlchemicComponent, 1);

        }

        OnItemAdd?.Invoke(AlchemicComponent, Inventory[AlchemicComponent]);
    }

    public bool HasElement(Component AlchemicComponent)
    {
        return Inventory.ContainsKey(AlchemicComponent);
    }

    public void UseElement(Component AlchemicComponent)
    {
        if (AlchemicComponent.RestoresSeconds == 0)
        {
            return;
        }

        GameManager.Instance.RestoreSeconds(AlchemicComponent.RestoresSeconds);
        RemoveElement(AlchemicComponent);

        if (alreadyKnownReceipts.Add(AlchemicComponent.ReceiptComponents.GUID))
        {
            LearnedReceipts.Add(AlchemicComponent.ReceiptComponents);
            OnLearnedReceipt?.Invoke(AlchemicComponent.ReceiptComponents);
        }
    }

    public void Deserialize(List<JSONReceipt> receipts)
    {
        LearnedReceipts.Clear();
        alreadyKnownReceipts.Clear();

        foreach (var receiptData in receipts)
        {
            var receipt = receiptData.Deserialize();

            if (alreadyKnownReceipts.Add(receipt.GUID))
            {
                LearnedReceipts.Add(receipt);
                OnLearnedReceipt?.Invoke(receipt);
            }
        }
    }

    public void Deserialize(List<JSONItem> items)
    {
        Inventory.Clear();
        foreach (var itemData in items)
        {
            var item = itemData.Component.Deserialize();
            Inventory.Add(item, itemData.Quantity);
            OnItemAdd?.Invoke(item, itemData.Quantity);
        }
    }

    public void RemoveElement(Component AlchemicComponent)
    {
        if (Inventory.ContainsKey(AlchemicComponent))
        {
            if (Inventory[AlchemicComponent] > 1)
            {
                Inventory[AlchemicComponent] -= 1;
                OnItemRemove?.Invoke(AlchemicComponent, Inventory[AlchemicComponent]);
            }
            else
            {
                Inventory.Remove(AlchemicComponent);
                OnItemRemove?.Invoke(AlchemicComponent, 0);
            }
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnTimeChange += FloatingTex;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnTimeChange -= FloatingTex;
    }

    public void FloatingTex(int time)
    {
        var t = Instantiate(FloatingText, transform.position, Quaternion.identity);
        t.GetComponent<TextMesh>().text = time.ToString("+0");
    }
}
