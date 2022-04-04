using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePerspective : MonoBehaviour
{
    public bool ShowETooltip = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != GameManager.Instance.MainPlayer.gameObject)
        {
            return;
        }

        float z = (transform.position.y < GameManager.Instance.MainPlayer.transform.position.y) ? -1 : -0.4f;
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
        if (ShowETooltip)
        {
            GameManager.Instance.ShowETooltip(transform.position, z);
        }
    }
}
