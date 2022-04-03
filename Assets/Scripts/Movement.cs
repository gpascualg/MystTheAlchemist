using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody2D rg;
    public float Speed = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!GameManager.Instance.IsInventoryOpen() && !GameManager.Instance.IsMixingOpen()) 
        { 
            Vector2 vel = Vector2.zero;
            if (Input.GetKey(KeyCode.W)) {
                vel.y += 1;
            }
            if (Input.GetKey(KeyCode.A)) {
                vel.x -= 1;
            }
            if (Input.GetKey(KeyCode.S)) {
                vel.y -= 1;
            }
            if (Input.GetKey(KeyCode.D)) {
                vel.x += 1;
            }

            rg.velocity = vel.normalized * Speed;
        }
    }
}
