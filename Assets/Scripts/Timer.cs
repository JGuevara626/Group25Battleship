using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timer = 20f;
    public TextMeshProUGUI timerText;
    void Start()
    {
        timerText.text = timer.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1 * Time.deltaTime;
        timerText.text = timer.ToString("0");
    }
}