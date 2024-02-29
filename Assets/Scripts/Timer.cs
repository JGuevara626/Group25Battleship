using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timer;
    public Text timerText;
    void Start()
    {
        timer = 20;
        timerText.text = timer.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1 * Time.deltaTime;
    }
}