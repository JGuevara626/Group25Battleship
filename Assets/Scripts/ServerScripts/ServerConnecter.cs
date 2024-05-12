using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class ServerConnecter : MonoBehaviourPunCallbacks
{
    public TMPro.TMP_InputField usernameField;
    public TextMeshProUGUI connectingText;

    public void OnClickEvent()
    {
        if (usernameField != null)
        {
            PhotonNetwork.NickName = usernameField.text;
            connectingText.text = "Searching Lobbies..";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("MatchmakingScreen");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
