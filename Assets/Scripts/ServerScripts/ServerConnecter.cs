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

    private string[] inappropriateWords = {
    "fuck",
    "shit",
    "asshole",
    "bitch",
    "cunt",
    "dick",
    "bastard",
    "whore",
    "motherfucker",
    "slut",
    "douchebag",
    "cocksucker",
    "twat",
    "prick",
    "cunt",
    "ass",
    "cock",
    "tits",
    "penis",
    "vagina",
    "anus",
    "blowjob",
    "wanker",
    "masturbate",
    "ejaculate",
    "semen",
    "clitoris",
    "testicles",
    "arsehole",
    "sodomy",
    "anal",
    "boner",
    "bugger",
    "crap",
    "damn",
    "fart",
    "gangbang",
    "hump",
    "jerkoff",
    "kinky",
    "mofo",
    "nipples",
    "orgasm",
    "perv",
    "queef",
    "rimjob",
    "skank",
    "smut",
    "taint",
    "titjob",
    "vulva",
    "wetdream",
    "x-rated",
    "yiff",
    "zoophilia",
    "felching",
    "fisting",
    "scat",
    "necrophilia"
};

    public void OnClickEvent()
    {
        string username = usernameField.text;
        if (IsUsernameValid(username))
        {
            PhotonNetwork.NickName = usernameField.text;
            connectingText.text = "Searching Lobbies..";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            connectingText.text = "appropriate or less than 8 characters ";
            return;
        }
    }

    private bool IsUsernameValid(string username)
    {
        if (username.Length > 8) // Check if username is longer than 8 characters
        {
            return false;
        }

        foreach (string word in inappropriateWords)
        {
            if (username.ToLower().Contains(word.ToLower()))
            {
                return false;
            }
        }
        return true;
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
