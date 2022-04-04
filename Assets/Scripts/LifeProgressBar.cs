using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeProgressBar : MonoBehaviour
{
    public static LifeProgressBar Instance;

    public Slider LifeProgress;
    public TMP_Text TextProgress;
    public float MaxTime;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        MaxTime = GameManager.Instance.getTime();
    }

    public void Subscribe()
    {
        GameManager.Instance.OnTimeReset += ResetTime;
        GameManager.Instance.OnTimeChange += UpdateTime;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnTimeReset -= ResetTime;
        GameManager.Instance.OnTimeChange -= UpdateTime;
    }

    public void ResetTime(float time)
    {
        MaxTime = time;
    }

    public void UpdateTime(int time)
    {
        float ActualTime = (GameManager.Instance.getTime() + (float)time);
        if (ActualTime > MaxTime) MaxTime = ActualTime;
        LifeProgress.value = (ActualTime / MaxTime);
        TextProgress.text = (LifeProgress.value * 100).ToString("0.0") + "%";
        ChangeColor(LifeProgress.value);
    }

    // Update is called once per frame
    public void Update()
    {
        float time = GameManager.Instance.getTime();
        LifeProgress.value = (time / MaxTime);
        TextProgress.text = (LifeProgress.value * 100).ToString("0.0") + "%";
        ChangeColor(LifeProgress.value);
    }

    public void ChangeColor(float progress)
    {
        if (LifeProgress.value < 0.10)
        {
            Color color = new Color(253f / 255f, 45f / 255f, 74f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        } else if (LifeProgress.value < 0.15)
        {
            Color color = new Color(217f / 255f, 106f / 255f, 55f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        }
        else if (LifeProgress.value < 0.20)
        {
            Color color = new Color(217f / 255f, 174f / 255f, 103f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        }
        else if (LifeProgress.value < 0.25)
        {
            Color color = new Color(217f / 255f, 190f / 255f, 103f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        }
        else if (LifeProgress.value < 0.30)
        {
            Color color = new Color(217f / 255f, 200f / 255f, 103f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        }
        else if (LifeProgress.value < 0.35)
        {
            Color color = new Color(217f / 255f, 216f / 255f, 103f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        }
        else if (LifeProgress.value < 0.40)
        {
            Color color = new Color(210f / 255f, 217f / 255f, 103f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        }
        else if (LifeProgress.value < 0.50)
        {
            Color color = new Color(188f / 255f, 217f / 255f, 103f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        }
        else if (LifeProgress.value < 0.75)
        {
            Color color = new Color(171f / 255f, 217f / 255f, 103f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        }
        else
        {
            Color color = new Color(147f / 255f, 217f / 255f, 103f / 255f);
            LifeProgress.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
        }
    }
}
