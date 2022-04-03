using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    private List<WorldComponent> nearCandidates = new List<WorldComponent>();
    public SerializedDictionary<Component, int> Inventory = new SerializedDictionary<Component, int>();
    public Action<Component, int> OnItemAdd;
    public Action<Component, int> OnItemRemove;

    private float nearestComponentDist = float.PositiveInfinity;
    private WorldComponent nearestComponent;

    private List<ReceiptComponents> learnedReceipts = new List<ReceiptComponents>();
    private HashSet<string> alreadyKnownReceipts = new HashSet<string>();
    public Action<ReceiptComponents> OnLearnedReceipt;

    public GameObject FloatingText;

    // Start is called before the first frame update
    void Start()
    {
        learnedReceipts.Add(Receipts.Instance.FindReceiptAt(0));
        alreadyKnownReceipts.Add(Receipts.Instance.FindReceiptAt(0).GUID);
        OnLearnedReceipt?.Invoke(Receipts.Instance.FindReceiptAt(0));
    }

    private float DistanceTo(WorldComponent comp)
    {
        return (comp.transform.position - transform.position).sqrMagnitude;
    }

    public void AddToNearest(WorldComponent component)
    {
        nearCandidates.Add(component);

        float distance = DistanceTo(component);
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
        nearCandidates.Remove(component);

        if (component == nearestComponent)
        {
            NormalNearest();
            nearestComponent = null;
            if (nearCandidates.Count > 0)
            {
                nearCandidates.Sort((x, y) => DistanceTo(x).CompareTo(DistanceTo(y)));
                nearestComponentDist = DistanceTo(nearCandidates[0]);
                nearestComponent = nearCandidates[0];
                HighlightNearest();
            }
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
                nearestComponent.Gather(this);
            }
        }

        if (nearCandidates.Count > 0)
        {
            nearCandidates.Sort((x, y) => DistanceTo(x).CompareTo(DistanceTo(y)));
            if (nearCandidates[0] != nearestComponent)
            {
                NormalNearest();
                nearestComponentDist = DistanceTo(nearCandidates[0]);
                nearestComponent = nearCandidates[0];
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
            learnedReceipts.Add(AlchemicComponent.ReceiptComponents);
            OnLearnedReceipt?.Invoke(AlchemicComponent.ReceiptComponents);
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

     private void OnDisable()
    {
        GameManager.Instance.OnTimeChange += FloatingTex;
    }

    public void FloatingTex(int time)
    {
        var t = Instantiate(FloatingText, transform.position, Quaternion.identity, transform);
        t.GetComponent<TextMesh>().text = time.ToString();
    }
}
