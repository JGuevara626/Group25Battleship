using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Battleship ship1;
    public Battleship ship2;
    public Battleship ship3;

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
        // if ship is already placed there, do nothing
        // otherwise if(shipsPlaced = 0, createShip(ship1), if shipsplaced = 1, createShip(ship2), if shipsPlaced = 2, createShip(ship3)
        // increment shipsPlaced;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        status = PlayerState.shipPlacement;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
