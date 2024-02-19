using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    public GameObject matchmakingPanel;
    public GameObject roomPanel;
    public TextMeshProUGUI roomName;
    [Space(10)]
    public RoomUIButton roomButton;
    List<RoomUIButton> roomButtonList = new List<RoomUIButton>();
    public Transform scrollContent;
    float nextUpdateTime;
    [Space(10)]
    public List<PlayerCard> playerCards = new List<PlayerCard>();
    public PlayerCard playerCardPrefab;
    public Transform playerCardTransform;
    public GameObject playButton;
    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }
    private void Update()
    {
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }

    public void OnClickPlay()
    {
        bool b = true;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if ((bool)player.Value.CustomProperties["isReady"] == false)
            {
                b = false;
            }
        }

        if (b)
        {
            PhotonNetwork.LoadLevel("GameVersus");
        }

    }

    public void OnClickCreateRoom()
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName, new RoomOptions() { MaxPlayers = 2, BroadcastPropsChangeToAll = true});
    }

    public override void OnJoinedRoom()
    {
        matchmakingPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Current Room: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        UpdatePlayerList();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        matchmakingPanel.SetActive(true );
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= nextUpdateTime)
        {
            foreach (RoomUIButton ruib in roomButtonList)
            {
                Destroy(ruib.gameObject);
            }
            roomButtonList.Clear();
            foreach (RoomInfo ri in roomList)
            {
                if (ri.PlayerCount == 0)
                {
                    continue;
                }
                else
                {
                    RoomUIButton ruib = Instantiate(roomButton, scrollContent);
                    ruib.SetRoomName(ri.Name);
                    roomButtonList.Add(ruib);
                }
            }

            nextUpdateTime = Time.time + 1.5f;
        }
    }

    void UpdatePlayerList()// how to find current player #if (player.Value == PhotonNetwork.LocalPlayer)
    {
        foreach(PlayerCard pc in playerCards)
        {
            Destroy(pc.gameObject);
        }
        playerCards.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)//sets the card info for each player #Ex player 1 name to card 1
        {
            PlayerCard pc = Instantiate(playerCardPrefab, playerCardTransform);
            pc.SetPlayerInfo(player.Value);
            playerCards.Add(pc);
            print("Player: " + player + " " + player.Value.NickName + " Has Entered");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
}
