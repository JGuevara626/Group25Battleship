using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsGroupHandler : MonoBehaviour
{
    public static CardsGroupHandler instance;
    public string CurrentAction;
    public GameObject mainButtons;
    public GameObject a, b;
    public bool actionsFull = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            //RectTransform rt = GetComponent<RectTransform>();
            //rt.anchoredPosition.Set(0, -40);
            //rt.anchorMax.Set(1, 0);
            //rt.anchorMin.Set(1, 0);
            mainButtons = b;
        }
        else
        {
            mainButtons = a;
        }


        this.gameObject.SetActive(false);
        mainButtons.SetActive(true);
    }

    public void SetAction(string action)
    {
        if (action != CurrentAction && action != "")
        {
            mainButtons.SetActive(false);
        }
        else
        {
            
            mainButtons.SetActive(true);
        }
        CurrentAction = action;
        GameManager.Instance.intakeAction(action);
    }

    public void handleCards(bool isfull)
    {
        if (isfull)
        {
            actionsFull = true;
            mainButtons.SetActive(false);
        }
        else
        {
            actionsFull = false;
            mainButtons.SetActive(true);
        }
    }
}
