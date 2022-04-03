using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiptsList : MonoBehaviour
{
    public static ReceiptsList Instance;

    public Transform Content;
    public GameObject ReceiptListPrefab;

    public void Awake()
    {
        Instance = this;
    }

    public void Subscribe()
    {
        GameManager.Instance.MainPlayer.OnLearnedReceipt += onLearnedReceipt;
    }

    private void OnDestroy()
    {
        GameManager.Instance.MainPlayer.OnLearnedReceipt -= onLearnedReceipt;
    }

    private void onLearnedReceipt(ReceiptComponents receipt)
    {
        var go = Instantiate(ReceiptListPrefab, Content);
        go.GetComponent<UIReceiptItem>().WithReceipt(receipt);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
