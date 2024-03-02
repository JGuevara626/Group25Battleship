using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class PlayerController : MonoBehaviour
{
    // characteristics of the player's turn

    // state(ship placement, selecting cards, selecting where to use cards, waiting for opponent/timer- game manager can set back to action select 
    public enum PlayerState //
    {
        shipPlacement,
        selectActions,
        selectActionLocation,
        waitPhase
    }
    public PlayerState status;
    // ship 1, ship 2, ship 3
    public List<Battleship> shipList = new List<Battleship>();

    public GameObject shipObj;

    // ships placed
    public int shipsPlaced;
    // ships alive/healthy count

    public int shipsAlive;
    // current ship selected
    public Battleship shipSelected;
    // cards in view - will refer to the card objects.

    // message to user
    public Text message;
    public int player = 0;
    public bool lookingForPOS = false;
    // lockInButton- needs to be created
    // cancel choice button - needs to be created in order to revert back from selectActionLocation state to selectActions state without locking in a choice.

    // functions

    // createShip

    void createShip(Battleship ship)
    {
        // instantiate the ship, making it a child of the player, and assigning "ship" to its script, so that we can access its characteristics. 
        ship = Instantiate(shipObj, this.transform).GetComponent<Battleship>();
    }
    // handle ship placement
    void handleShipPlacement()
    {
        // will be called on click during shipPlacement state
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //print(Mathf.RoundToInt(mouseWorldPos.x) + " " + Mathf.RoundToInt(mouseWorldPos.y));
        Vector2 v2 = new Vector2(Mathf.RoundToInt(mouseWorldPos.x), Mathf.RoundToInt(mouseWorldPos.y));
        // if ship is already placed there, do nothing
        bool b = false;
        if(player == 1)
        {
            b = true;
        }
        Tile tilePos = GridManager.instance.GetTilePOS(v2, b);
        if (tilePos != null && tilePos.Occupied == false)
        {
            GameObject ship = Instantiate(shipObj, this.transform);
            ship.GetComponent<Battleship>().player = player;
            tilePos.setUnit(ship.GetComponent<Battleship>());
            shipsPlaced++;
            shipList.Add(ship.GetComponent<Battleship>());
        }

        // otherwise if(shipsPlaced = 0, createShip(ship1), if shipsplaced = 1, createShip(ship2), if shipsPlaced = 2, createShip(ship3)
        // increment shipsPlaced;
        if (shipsPlaced == 3)
        {
            ChangeState(PlayerState.waitPhase);
            foreach (var ship in shipList)
            {
                ship.choice = "";
            }
        }
        else
        {
            lookingForPOS=true;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ChangeState(PlayerState.shipPlacement);
        //lookingForPOS=true;
    }

    // Update is called once per frame
    void Update()
    {
        print("Local Player Actor Number " + PhotonNetwork.LocalPlayer.ActorNumber);
        if (lookingForPOS && Input.GetMouseButtonDown(0) && player == 1)
        {
            lookingForPOS=false;
            handleShipPlacement();
        }
    }

    public void ChangeState(PlayerState pstate)
    {
        status = pstate;
        switch (pstate)
        {
            case PlayerState.shipPlacement:
                lookingForPOS = true;
                break;
            case PlayerState.waitPhase:
                break;
        }
    }
}   

