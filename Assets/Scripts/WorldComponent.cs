using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class WorldComponent : MonoBehaviour
{
    public Component AlchemicComponent;

    public int WorldId { get; set; }

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
        GameManager.Instance.PlayFX(GameManager.Instance.PickupSound);
        player.AddComponent(AlchemicComponent);
        player.RemoveFromNearest(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = GameManager.Instance.MainPlayer;
        if (collision.gameObject == player.gameObject)
        {
            player.AddToNearest(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = GameManager.Instance.MainPlayer;
        if (collision.gameObject == player.gameObject)
        {
            player.RemoveFromNearest(this);
        }
    }
}
