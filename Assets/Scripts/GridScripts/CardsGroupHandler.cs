using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            mainButtons.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {

            mainButtons.transform.GetChild(1).gameObject.SetActive(true);
        }
        CurrentAction = action;
        GameManager.Instance.intakeAction(action);
    }

    public void handleCards()
    {
        CurrentAction = "";

        if(GameManager.Instance.checkShipsOperated() && !actionsFull) //if all player ships are lockedin
        {
            actionsFull = true;
            mainButtons.SetActive(false);
            GameManager.Instance.changeToWait(PlayerController.PlayerState.waitPhase);
        }
        else //call after all actions are done to reset
        {
            actionsFull = false;
            mainButtons.SetActive(true);
            mainButtons.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
