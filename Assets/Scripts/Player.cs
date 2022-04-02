using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public List<WorldComponent> NearCandidates = new List<WorldComponent>();
    public SerializedDictionary<Component, int> Inventory = new SerializedDictionary<Component, int>();
    public Action<Component> OnItemAdd;
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

    public void UseElement(Component AlchemicComponent)
    {
        if (Inventory.ContainsKey(AlchemicComponent))
        {
            if (Inventory[AlchemicComponent] > 1)
            {
                OnItemRemove?.Invoke(AlchemicComponent, Inventory[AlchemicComponent]);
                Inventory[AlchemicComponent] -= 1;
            }
            else
            {
                OnItemRemove?.Invoke(AlchemicComponent, 0);
                Inventory.Remove(AlchemicComponent);

            }
        }
    }
}
