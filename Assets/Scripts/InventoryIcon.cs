using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryIcon : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("??");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Open inventory");
        if(GameManager.Instance.IsInventoryOpen())
        {
            GameManager.Instance.CloseInventory();
        }
        else
        {
            GameManager.Instance.OpenInventory();
        }
    }
}
