using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    //public List<Tile> radarships = new List<Tile>();
    private List<Tile> Scannedships = new List<Tile>();
    private Battleship tokenShip;



    // instantiate touchControls
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;

    private TouchControls touchControls;

    private new void OnEnable()
    {
        touchControls.Enable();
    }
    private new void OnDisable()
    {
        touchControls.Disable();
    }
    private void StartTouch(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null) OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        Debug.Log("Game manager script detected start touch: " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
    }
    private void EndTouch(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null) OnEndTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
        Debug.Log("Game manager script detected end touch: " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
    }

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

        StartCoroutine(TurnCoroutine(turnOrderShips));
        // still need to decide on this. Current order decided on is just alternating ships, but this might be better
        // both players' defending ships go-> 
        // both players' moving ships go-> 
        // both players attacking ships go in alternating order(player 1 will go first on turn 1, player 2 will go first on turn 2.....) ->
        // both players' ships scan -> end turn

        // change state to choiceSelection or battleAnimation based on if players have ships remaining

        //reset states and timer
    }

    IEnumerator TurnCoroutine(List<Battleship> turnOrderShips)
    {

        foreach (Battleship ship in turnOrderShips)
        {
            //if (ship.destroyed) { continue; }
            string s = ("Player " + ship.player + "'s " + ship.name +" Turn").ToString();
            timer.displayText(s);
            yield return new WaitForSeconds(1);
            switch (ship.choice)
            {
                case "Move":
                    moveShip(ship);
                    break;
                case "Radar":
                    radarScanning(ship.player);
                    break;
                case "Fire":
                    shootCannon(ship);
                    break;
            }
            yield return new WaitForSeconds(1);
        }

        resetAll(turnOrderShips);

        if (player1.checkIfDefeated() || player2.checkIfDefeated())
        {
            ChangeState(gameState.gameOver);
        }
        else
        {
            ChangeState(gameState.choiceSelection);
        }

    }

    void resetAll(List<Battleship> turnOrderShips)
    {
        foreach (Battleship ship in turnOrderShips)
        {
            
            if (PhotonNetwork.LocalPlayer.ActorNumber == ship.player)
            {
                ship.resetChoice();
                shipToController(ship);
            }
        }
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

    void HandleShipAction(string type)
    {
        //Battleship bs = new Battleship();
        switch (PhotonNetwork.LocalPlayer.ActorNumber)
        {
            case 1:
                tokenShip = player1.getShip(type);
                break;
            case 2:
                tokenShip = player2.getShip(type);
                break;
        }

        bool b = false;

        if (tokenShip != null)
        {
            print("Selected Ship: " +  tokenShip.name);
            switch(heldAction)
            {
                case "Move":
                    tokenShip.setChoice("Move");
                    b = true;
                    break;
                case "Radar":
                    tokenShip.setChoice("Radar");
                    b = true;
                    break;
                case "Shield":
                    tokenShip.setChoice("Shield");
                    b = true;
                    break;
                case "Fire":
                    tokenShip.setChoice("Fire");
                    b = true;
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

    public void radarScanning(int isPlayer1)
    {
        //callShipLocation();
        List<Tile> radarships = new List<Tile>();
        if (isPlayer1 == 1 && PhotonNetwork.LocalPlayer.ActorNumber == isPlayer1)
        {
            clearScan(PhotonNetwork.LocalPlayer.ActorNumber);
            foreach (Battleship bs in player2.shipList)
            {
                radarships.Add(bs.OccupiedTile);
                bs.OccupiedTile.GetComponent<Tile>().SetHighlight(true);
                Scannedships.Add(bs.OccupiedTile);
            }
        }
        else if(isPlayer1 == 2 && PhotonNetwork.LocalPlayer.ActorNumber == isPlayer1)
        {
            clearScan(PhotonNetwork.LocalPlayer.ActorNumber);
            foreach (Battleship bs in player1.shipList)
            {
                radarships.Add(bs.OccupiedTile);
                bs.OccupiedTile.GetComponent<Tile>().SetHighlight(true);
                Scannedships.Add(bs.OccupiedTile);
            }
        }
        

    }

    void clearScan(int n)
    {
        if (n == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            foreach(Tile go in Scannedships)
            {
                go.GetComponent<Tile>().SetHighlight(false);
            }
            Scannedships.Clear();
        }
    }

    //shootCannon(shotFrom, shotTo)
    void shootCannon(Battleship bs)
    {
        // spawn a cannonball at shootFrom and make its target shotTo
        bool hitShip = false;
        Battleship bs2 = null;
        if (bs.player == 1)
        {
            foreach(Battleship b in player2.shipList)
            {
                if(bs.target == b.position)
                {
                    bs2 = b;
                    hitShip = true;
                    break;
                }
            }
        }
        else
        {
            foreach (Battleship b in player1.shipList)
            {
                if (bs.target == b.position)
                {
                    bs2 = b;
                    hitShip = true;
                    break;
                }
            }
        }

        if (hitShip)
        {
            if (bs2.shield == true)
            {
                bs2.shield = false;
                string s = ("Player " + bs2.player + "'s " + bs2.name + " Defended The Attack!").ToString();
                timer.displayText(s);
            }
            else if(bs2.destroyed)
            {
                string s = ("Player " + bs2.player + "'s Ship Is Destroyed").ToString();
                timer.displayText(s);
            }
            else
            {
                string s = ("Player " + bs2.player + "'s " + bs2.name + " Has Been Hit!").ToString();
                timer.displayText(s);
                bs2.killSelf();
                //bs2.resetChoice();
            }
        }
        else
        {
            string s = ("Player " + bs.player + "'s " + bs.name + " Missed!").ToString();
            timer.displayText(s);
        }

        Tile t = null;
        if (bs.player == 1)
        {
            t = GridManager.instance.GetTilePOS(bs.target, false);
        }
        else
        {
            t = GridManager.instance.GetTilePOS(bs.target, true);
        }
        t.SetTargeted(false);

        //bs.resetChoice();

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
        //bs.resetChoice();

    }
    //
    private void Awake()
    {
        Instance = this;
        touchControls = new TouchControls();
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
                //string s = "Place Your Ships!";
                timer.displayText("Place Your Ships!");
                break;
            case gameState.choiceSelection:
                changeToWait(PlayerState.selectActions);
                CardsGroupHandler.instance.handleCards();
                timer.startCountdown(60f);
                break; 
            case gameState.battleAnimation:
                HandleTurnOrder();
                break;
            case gameState.gameOver:
                gameoversceneChange(PhotonNetwork.LocalPlayer.ActorNumber);
                break;
            default:
                break;
        }
    }

    void gameoversceneChange(int playerint)
    {
        if(playerint == 1)
        {
            if(player1.checkIfDefeated())
            {
                SceneManager.LoadScene("LoseScreen");
            }
            else
            {
                SceneManager.LoadScene("WinScreen");
            }
        }
        else
        {
            if (player2.checkIfDefeated())
            {
                SceneManager.LoadScene("LoseScreen");
            }
            else
            {
                SceneManager.LoadScene("WinScreen");
            }
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
    public void UpdateSingleShip(Vector2 pos, Vector2 target, string action, int shipIndex, int playerIndex, bool shielded, bool destroyed)
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
            battleS.shield = shielded;
            battleS.destroyed = destroyed;
            pC.shipList.Add(battleS);

            if (playerIndex != PhotonNetwork.LocalPlayer.ActorNumber) //hides enemy ships of local player
            {
                ship.GetComponent<Renderer>().enabled = false;
                ship.name = ("EnemyShip#" + shipIndex);
            }
            else
            {
                ship.name = ("Ship#" + shipIndex);
            }
        }
        else
        {
            pC.shipList[shipIndex].position = pos;
            pC.shipList[shipIndex].target = target;
            pC.shipList[shipIndex].choice = action;
            pC.shipList[shipIndex].shield = shielded;
            pC.shipList[shipIndex].destroyed = destroyed;
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
            HandleShipAction("mouse");
        }
        if (searchingTile && OnStartTouch != null)
        {
            searchingTile = false;
            HandleShipAction("touch");
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

    //public void callShipLocation()
    //{
    //    photonView.RPC("sendShips", RpcTarget.All);
    //}

    public void changeToWait(PlayerState ps)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            player1.status = ps;
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            player2.status = ps;
        }

        sendPC(PhotonNetwork.LocalPlayer.ActorNumber);
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
                player2.sendShipData(b, t);
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

    public void timerDone()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            foreach (Battleship bs in player1.shipList)
            {
                if (!bs.lockChoice)
                {
                    bs.lockChoice = true;
                }
            }
        }
        else
        {
            foreach (Battleship bs in player2.shipList)
            {
                if (!bs.lockChoice)
                {
                    bs.lockChoice = true;
                }
            }
        }
        changeToWait(PlayerState.waitPhase);
    }

    public Tile getEnemyTileSection(Vector2 v2)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            return GridManager.instance.GetTilePOS(v2, false);
        }
        else
        {
            return GridManager.instance.GetTilePOS(v2, true);
        }
    }
}
