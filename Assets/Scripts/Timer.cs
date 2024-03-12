using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timer = 20f;
    public TextMeshProUGUI timerText;
    public bool timerStarted = false;
    void Start()
    {
        //timerText.text = timer.ToString();
        //GetComponent<Slider>().maxValue = timer;
        //GetComponent<Slider>().value = timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerStarted)
        {
            timer -= 1 * Time.deltaTime;
            timerText.text =("Assign Actions: " + timer.ToString("0"));
            GetComponent<Slider>().value = timer;
            if (timer <= 0)
            {
                timerStarted = false;
                GameManager.Instance.timerDone();
            }
        }
    }

    public void startCountdown(float time)
    {
        timer = time;
        timerStarted = true;
        GetComponent<Slider>().maxValue = timer;
        GetComponent<Slider>().value = timer;
    }

    public void displayText(string t)
    {
        timerStarted = false;
        timerText.text = t;
        GetComponent<Slider>().maxValue = timer;
        GetComponent<Slider>().value = timer;
    }

}