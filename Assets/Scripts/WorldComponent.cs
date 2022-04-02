using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class WorldComponent : MonoBehaviour
{
    public Component AlchemicComponent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Gather(Player player)
    {
        if (player.Inventory.ContainsKey(AlchemicComponent)){
            player.Inventory[AlchemicComponent] += 1;
        } else {
            player.Inventory.Add(AlchemicComponent, 1);

        }

        player.NearCandidates.Remove(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = GameManager.Instance.MainPlayer;
        if (collision.gameObject == player.gameObject)
        {
            player.NearCandidates.Add(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = GameManager.Instance.MainPlayer;
        if (collision.gameObject == player.gameObject)
        {
            player.NearCandidates.Remove(this);
        }
    }
}
