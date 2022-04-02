using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    public Slider LifeProgress;
    public TMP_Text TextProgress;
    public float MaxTime;

    // Start is called before the first frame update
    void Start()
    {
        MaxTime = 300;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnTimeChange += UpdateTime;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnTimeChange -= UpdateTime;
    }

    public void UpdateTime(int time)
    {
        float ActualTime = (GameManager.Instance.getTime() + (float)time);
        if (ActualTime > MaxTime) MaxTime = ActualTime;
        LifeProgress.value = (ActualTime / MaxTime);
        TextProgress.text = (LifeProgress.value * 100).ToString("0.0") + "%";
    }

    // Update is called once per frame
    public void Update()
    {
        float time = GameManager.Instance.getTime();
        LifeProgress.value = (time / MaxTime);
        TextProgress.text = (LifeProgress.value * 100).ToString("0.0") + "%";
    }
}
