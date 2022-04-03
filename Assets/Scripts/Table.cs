using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool hasPlayer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (hasPlayer && Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance.IsMixingOpen())
            {
                GameManager.Instance.CloseInventory();
                GameManager.Instance.CloseMixing();
            }
            else
            {
                GameManager.Instance.OpenInventory();
                GameManager.Instance.OpenMixing();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != GameManager.Instance.MainPlayer.gameObject)
        {
            return;
        }

        spriteRenderer.material = GameManager.Instance.OutlineMaterial;
        hasPlayer = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != GameManager.Instance.MainPlayer.gameObject)
        {
            return;
        }

        float z = (transform.position.y + 0.15 < GameManager.Instance.MainPlayer.transform.position.y) ? -1 : -0.5f;
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != GameManager.Instance.MainPlayer.gameObject)
        {
            return;
        }

        spriteRenderer.material = GameManager.Instance.NormalMaterial;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        hasPlayer = false;
    }
}
