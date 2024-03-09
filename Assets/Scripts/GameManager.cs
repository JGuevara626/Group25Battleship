using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
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

    // handle result of turn
    void HandleTurnOrder()
    {
        List<Battleship> turnOrderShips = new List<Battleship>();

        for (int i = 0; i < 3; i++){

            if (!player1.shipList[i].destroyed)
            {
                turnOrderShips.Add(player1.shipList[i]);
            }

            if (!player2.shipList[i].destroyed)
            {
                turnOrderShips.Add(player2.shipList[i]);
            }
        } 

        foreach (Battleship ship in turnOrderShips)
        {
            StartCoroutine(TurnCoroutine(ship));
        }
        // still need to decide on this. Current order decided on is just alternating ships, but this might be better
        // both players' defending ships go-> 
        // both players' moving ships go-> 
        // both players attacking ships go in alternating order(player 1 will go first on turn 1, player 2 will go first on turn 2.....) ->
        // both players' ships scan -> end turn
        
        // change state to choiceSelection or battleAnimation based on if players have ships remaining


        if(player1.checkIfDefeated() ||  player2.checkIfDefeated())
        {
            ChangeState(gameState.gameOver);
        }
        else
        {
            ChangeState(gameState.choiceSelection);
        }
        //reset states and timer
    }

    IEnumerator TurnCoroutine(Battleship ship)
    {
        yield return new WaitForSeconds(1);
        switch (ship.choice)
        {
            case "Move":
                moveShip(ship);
                break;
        }
        yield return new WaitForSeconds(2);
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

        if (tokenShip != null)
        {
            switch(heldAction)
            {
                case "Move":
                    tokenShip.setChoice("Move");
                    b = true;
                    break;
                case "Radar":
                    tokenShip.setChoice("Radar");
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
        }

        if (!b)
        {
            searchingTile = true;
        }
        else
        {
            tokenShip = null;
        }

        // if ship is not alive, just skip 
        // if ship action is shoot, shootCannon(pl)
        // if ship action is move, moveShip()
        // if ship action is defend, activate shield
        // if ship action is default, shoot a cannonball at a random square?

        // wipe ship's selection clean
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
        if (bs.player == 1)//do these when movement is played
        {
            Tile tilePos = GridManager.instance.GetTilePOS(bs.target, true);
            tilePos.setUnit(bs);
        }
        else
        {
            Tile tilePos = GridManager.instance.GetTilePOS(bs.target, false);
            tilePos.setUnit(bs);
        }
        bs.target = new Vector2(0,0);
        bs.resetChoice();

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
                HandleTurnOrder();
                break;
            case gameState.gameOver:
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
            ChangeState(gameState.choiceSelection);
        }

        if(player1.status == PlayerState.waitPhase && player2.status == PlayerState.waitPhase)
        {
            ChangeState(gameState.battleAnimation);
        }

    }

    [PunRPC]
    public void UpdateSingleShip(Vector2 pos, Vector2 target, string action, int shipIndex, int playerIndex)
    {
        PlayerController pC = null;
        switch (playerIndex)
        {
            case 1:
                pC = player1;
                break;
            case 2:
                pC = player2;
                break;
        }

        if ((pC.shipList.Count - 1) < shipIndex)
        {
            GameObject ship = Instantiate(bs, this.transform);
            if(playerIndex != 1) { ship.transform.rotation = Quaternion.Euler(0, 180, 0); }
            Battleship battleS = ship.GetComponent<Battleship>();

            if (playerIndex == 1)
            {
                Tile tilePos = GridManager.instance.GetTilePOS(pos, true);
                tilePos.setUnit(battleS);
            }
            else
            {
                Tile tilePos = GridManager.instance.GetTilePOS(pos, false);
                tilePos.setUnit(battleS);
            }
            battleS.player = playerIndex;
            battleS.position = pos;
            battleS.target = target;
            battleS.choice = action;
            pC.shipList.Add(battleS);

            if (playerIndex != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                ship.GetComponent<Renderer>().enabled = false;
                ship.name = ("EnemyShip#" + shipIndex);
            }
        }
        else
        {
            pC.shipList[shipIndex].position = pos;
            pC.shipList[shipIndex].target = target;
            pC.shipList[shipIndex].choice = action;
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
            photonView.RPC("UpdatePlayerController", RpcTarget.All, player1.status, n);
        }
        else if (n == 2)
        {
            photonView.RPC("UpdatePlayerController", RpcTarget.All, player2.status, n);
        }
        
    }

    public void callShipLocation()
    {
        photonView.RPC("sendShips", RpcTarget.All);
    }

    public void changeToWait()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            player1.status = PlayerState.waitPhase;
            sendPC(1);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            player1.status = PlayerState.waitPhase;
            sendPC(2);
        }

    }

    public void shipToController(Battleship b)
    {
        switch (PhotonNetwork.LocalPlayer.ActorNumber)
        {
            case 1:
                int l = player1.locateShipInList(b);
                player1.sendShipData(b, l);
                break;
            case 2:
                int t = player2.locateShipInList(b);
                player1.sendShipData(b, t);
                break;
        }
    }

    public bool checkShipsOperated()
    {
        bool b = true;
        if(PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            foreach (Battleship bs in player1.shipList)
            {
                if (!bs.destroyed && !bs.lockChoice)
                {
                    b = false;
                    break;
                }
            }
        }
        else
        {
            foreach (Battleship bs in player2.shipList)
            {
                if (!bs.destroyed && !bs.lockChoice)
                {
                    b = false;
                    break;
                }
            }
        }
        return b;
    }
}
