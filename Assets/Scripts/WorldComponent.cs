using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class WorldComponent : MonoBehaviour
{
    public Component AlchemicComponent;
    private float untilButterflyCheck;
    private const float BUTTERFLY_INTERVAL = 10.0f;
    private SpriteRenderer spriteRenderer;

    public int WorldId { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        untilButterflyCheck = Random.Range(3, BUTTERFLY_INTERVAL);
    }

    // Update is called once per frame
    void Update()
    {
        if (AlchemicComponent.ComponentType == ComponentType.Flower && spriteRenderer.isVisible)
        {
            untilButterflyCheck -= Time.deltaTime;
            if (untilButterflyCheck <= 0)
            {
                untilButterflyCheck = BUTTERFLY_INTERVAL;
                if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f)
                {
                    GameManager.Instance.SpawnButterfly(transform.position);
                }
            }
        }
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
