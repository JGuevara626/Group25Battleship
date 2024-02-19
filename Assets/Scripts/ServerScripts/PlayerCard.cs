using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerName;
    public Player currentPlayer;
    public Button ReadyButton;
    public bool isReady = false;
    public Image readyIMG;

    ExitGames.Client.Photon.Hashtable playerProperty = new ExitGames.Client.Photon.Hashtable();

    public void SetPlayerInfo(Player p)
    {
        playerName.text = p.NickName;
        currentPlayer = p;
        if (p == PhotonNetwork.LocalPlayer)
        {
            ReadyButton.interactable = true;
        }
        updatePlayerCard(currentPlayer);
    }

    public void setReady()
    {
        isReady = !isReady;
        if ((bool)playerProperty["isReady"] != isReady)
        {
            playerProperty["isReady"] = isReady;
        }
        changeIMG(isReady);

        PhotonNetwork.SetPlayerCustomProperties(playerProperty);
    }

    public void changeIMG(bool b)
    {
        switch (b)
        {
            case true:
                readyIMG.color = Color.green;
                break;
            case false:
                readyIMG.color = Color.red;
                break;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (currentPlayer == targetPlayer)
        {
            updatePlayerCard(targetPlayer);
        }
    }

    void updatePlayerCard(Player player)
    {
        if (player.CustomProperties.ContainsKey("isReady"))
        {
            changeIMG((bool)player.CustomProperties["isReady"]);
            playerProperty["isReady"] = (bool)player.CustomProperties["isReady"];
        }
        else
        {
            playerProperty["isReady"] = false;
        }
    }
}
