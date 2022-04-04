using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleAudio : MonoBehaviour, IPointerClickHandler
{
    public bool IsFX;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsFX)
        {
            GameManager.Instance.ToggleFXSound();
        }
        else
        {
            GameManager.Instance.ToggleBGSound();
        }
    }
}