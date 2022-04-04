using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    public float Speed;
    private float lifetime;
    private float despawnTime;
    private Vector3 origin;
    private float movementLeft;
    private Vector3 direction;

    private const float DESPAWN_ANIMATION_TIME = 5.1f;

    private Animator animator;

    void Start()
    {
        lifetime = Random.Range(5, 50);
        despawnTime = DESPAWN_ANIMATION_TIME + 1e-6f;
        origin = transform.position;
        movementLeft = 0.0f;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            if (despawnTime >= DESPAWN_ANIMATION_TIME)
            {
                animator.SetBool("Despawn", true);
            }

            despawnTime -= Time.deltaTime;
            if (despawnTime <= 0)
            {
                GameManager.Instance.DespawnButterfly(gameObject);
            }
            return;
        }

        if (movementLeft <= 0)
        {
            Vector3 target = origin + new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), 0.0f);
            direction = (target - transform.position).normalized;
            movementLeft = (target - transform.position).magnitude / Speed;
        }

        movementLeft -= lifetime;
        Vector3 nudgedDirection = direction + new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), 0);
        transform.position = transform.position + nudgedDirection.normalized * Speed * Time.deltaTime;
    }
}
