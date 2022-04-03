using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public Rigidbody2D rg;
    public SpriteRenderer sr;
    public float Speed = 1.5f;

    public Sprite MoveUp;
    public Sprite MoveDown;
    public Sprite MoveRight;
    public Sprite MoveLeft;


    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 vel = Vector2.zero;
        if(!GameManager.Instance.IsMixingOpen()) 
        { 
            if (Input.GetKey(KeyCode.W)) {
                vel.y += 1;
                sr.sprite = MoveUp;
            }
            if (Input.GetKey(KeyCode.A)) {
                vel.x -= 1;
                sr.sprite = MoveLeft;
            }
            if (Input.GetKey(KeyCode.S)) {
                vel.y -= 1;
                sr.sprite = MoveDown;
            }
            if (Input.GetKey(KeyCode.D)) {
                vel.x += 1;
                sr.sprite = MoveRight;
            }

            rg.velocity = vel.normalized * Speed;
        }
        else
        {
            rg.velocity = vel.normalized * Speed;
        }
    }
}
