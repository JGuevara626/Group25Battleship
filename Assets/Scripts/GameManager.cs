using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using static PlayerController;

public class GameManager : MonoBehaviourPunCallbacks
{
    //characteristics of game manager
    public static GameManager Instance;
    //state(ship placement, choice selection, animation/turn playout phase, game over)
    public enum gameState
    {
        shipPlacement,
        choiceSelection,
        battleAnimation,
        gameOver
    }
    public gameState state;
    //timer(will be put in once a timerScript is created
    public Timer timer;
    // references to both players ships to access their characteristics
    public PlayerController player1;//private?
    public PlayerController player2;//private?
    public GameObject bs;
    public GameObject CardsGroupUI;
    private bool searchingTile = false;
    private string heldAction = "";
    public List<Tile> radarships = new List<Tile>();
    private List<GameObject> Scannedships = new List<GameObject>();
    private Battleship tokenShip;
    //functions
    ExitGames.Client.Photon.Hashtable playerProperty = new ExitGames.Client.Photon.Hashtable();
    // handle result of turn
    void HandleTurnOrder()
    {
        // still need to decide on this. Current order decided on is just alternating ships, but this might be better
        // both players' defending ships go-> 
        // both players' moving ships go-> 
        // both players attacking ships go in alternating order(player 1 will go first on turn 1, player 2 will go first on turn 2.....) ->
        // both players' ships scan -> end turn
        
        // change state to choiceSelection or battleAnimation based on if players have ships remaining

        //reset states and timer
    }

    public void intakeAction(string action)
    {
        if (action != "")
        {
            searchingTile = true;
        }
        else
        {
            searchingTile = false;
        }
        heldAction = action;
    }

    void HandleShipAction()
    {
        //Battleship bs = new Battleship();
        switch (PhotonNetwork.LocalPlayer.ActorNumber)
        {
            case 1:
                tokenShip = player1.getShip();
                break;
            case 2:
                tokenShip = player2.getShip();
                break;
        }

        bool b = false;
        switch(heldAction)
        {
            case "Move":
                tokenShip.choice = "Move";
                b = true;
                break;
            case "Radar":
                tokenShip.choice = "Radar";
                b = true;
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                {
                    GameManager.Instance.radarScanning(true);
                }
                else
                {
                    GameManager.Instance.radarScanning(false);
                }
                    break;
        }

        // if ship is not alive, just skip 
        // if ship action is shoot, shootCannon(pl)
        // if ship action is move, moveShip()
        // if ship action is defend, activate shield
        // if ship action is default, shoot a cannonball at a random square?

        // wipe ship's selection clean
        if (!b)
        {
            searchingTile = true;
        }
    }

    public void radarScanning(bool isPlayer1)
    {
        callShipLocation();
        PlayerController pc = gameObject.AddComponent<PlayerController>();
        clearScan();
        if (isPlayer1)
        {
            foreach( Battleship bs in player2.shipList)
            {
                radarships.Add(bs.OccupiedTile);
            }
        }
        else
        {
            foreach (Battleship bs in player1.shipList)
            {
                radarships.Add(bs.OccupiedTile);
            }
        }

        foreach (Tile t in radarships)
        {
            GameObject go = Instantiate(bs, t.transform);
            go.GetComponent<SpriteRenderer>().color = Color.green;
            Scannedships.Add(go);
        }
        

    }

    void clearScan()
    {
        foreach (Tile t in radarships)
        {
            Destroy(t.gameObject);
        }
        radarships.Clear();
        foreach (GameObject go in Scannedships)
        {
            DestroyImmediate(go);
        }
        Scannedships.Clear();
    }

    //shootCannon(shotFrom, shotTo)
    void shootCannon()
    {
        // spawn a cannonball at shootFrom and make its target shotTo
    }
    //moveShip(moveTo)'
    void moveShip(Battleship bs)// figure out what data type a ship's position is
    {
        // position = newPosition;
    }
    //
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player1 = gameObject.AddComponent<PlayerController>();
        player2 = gameObject.AddComponent<PlayerController>();
        player1.player = 1;
        player1.shipObj = bs;
        player2.player = 2;
        player2.shipObj = bs;
        ChangeState(gameState.shipPlacement);
        print(photonView);
    }

    public void ChangeState(gameState gstate) 
    {
        state = gstate;
        switch (gstate)
        {
            case gameState.shipPlacement:
                GridManager.instance.GenerateGrid();

                break;
            case gameState.choiceSelection:
                break; 
            case gameState.battleAnimation:
                break;
            default:
                break;
        }
    }

    [PunRPC]
    public void UpdatePlayerController(PlayerState pc, int n)
    {
        switch (n)
        {
            case 1:
                player1.status = pc;

                break;
            case 2:
                player2.status = pc;
                break;
        }

        if(player1.status == PlayerState.placementReady && player2.status == PlayerState.placementReady)
        {
            CardsGroupUI.SetActive(true);
            player1.ChangeState(PlayerState.selectActions);
            player2.ChangeState(PlayerState.selectActions);
        }

    }

    [PunRPC]
    public void updateShipCoords(int n, List<Battleship> shipList)
    {
        switch (n)
        {
            case 1:
                player1.shipList = shipList;
                break;
            case 2:
                player2.shipList = shipList;
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // if timer expires or both players are finished locking in their choices, then change states to the battleAnimation state.
        //if (timer.timer == 0f || (player1.status == PlayerController.PlayerState.waitPhase && player2.status == PlayerController.PlayerState.waitPhase))
        //{ 
        //    HandleTurnOrder();
        //}

        if(searchingTile && Input.GetMouseButtonDown(0))
        {
            searchingTile = false;
            HandleShipAction();
        }

    }

    public void sendPC(int n)
    {

        if (n == 1)
        {
            print("if player1 is true:" + player1.isActiveAndEnabled);
            photonView.RPC("UpdatePlayerController", RpcTarget.All, player1.status, n);
        }
        else if (n == 2)
        {
            photonView.RPC("UpdatePlayerController", RpcTarget.All, player2.status, n);
        }
        
    }

    [PunRPC]
    public void sendShips()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            photonView.RPC("updateShipCoords", RpcTarget.All, 1, player1.shipList);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            photonView.RPC("updateShipCoords", RpcTarget.All, 2,player2.shipList);
        }
    }

    public void callShipLocation()
    {
        photonView.RPC("sendShips", RpcTarget.All);
    }


    //public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    //{
    //    if (PhotonNetwork.LocalPlayer == targetPlayer)
    //    {
    //        updateClient(targetPlayer);
    //    }
    //}

    //public void updateClient(Player targetPlayer)
    //{
    //    if (targetPlayer.CustomProperties.ContainsKey("pc1"))
    //    {
    //        player1 = (PlayerController)targetPlayer.CustomProperties["pc1"];
    //        playerProperty["pc1"] = (PlayerController)targetPlayer.CustomProperties["pc1"];
    //    }
    //    else if (targetPlayer.CustomProperties.ContainsKey("pc2"))
    //    {
    //        player2 = (PlayerController)targetPlayer.CustomProperties["pc2"];
    //        playerProperty["pc2"] = (PlayerController)targetPlayer.CustomProperties["pc2"];
    //    }
    //}
}
