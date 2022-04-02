using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public List<WorldComponent> NearCandidates = new List<WorldComponent>();
    public SerializedDictionary<Component, int> Inventory = new SerializedDictionary<Component, int>();
    public Action<Component, int> OnItemAdd;
    public Action<Component, int> OnItemRemove;

    // Start is called before the first frame update
    void Start()
    {
           
    }

    private float DistanceTo(WorldComponent comp)
    {
        return (comp.transform.position - transform.position).sqrMagnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (NearCandidates.Count > 0)
            {
                NearCandidates.Sort((x, y) => DistanceTo(x).CompareTo(DistanceTo(y)));
                NearCandidates[0].Gather(this);
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

    public void UseElement(Component AlchemicComponent)
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
}
