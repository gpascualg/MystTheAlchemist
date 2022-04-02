using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Text;

    public void Show(Component component)
    {
        Name.text = component.Name;
        Text.text = component.Description;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
