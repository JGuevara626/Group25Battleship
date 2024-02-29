using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomUIButton : MonoBehaviour
{
    public TextMeshProUGUI roomName;
    MatchmakingManager manager;

    private void Start()
    {
        manager = FindObjectOfType<MatchmakingManager>();
    }
    public void SetRoomName(string rN)
    {
        roomName.text = rN;
    }
    public void OnClickButton()
    {
        manager.JoinRoom(roomName.text);
    }
}
