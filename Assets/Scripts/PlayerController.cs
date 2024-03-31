using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class PlayerController : MonoBehaviourPunCallbacks
{
    // characteristics of the player's turn

    // state(ship placement, selecting cards, selecting where to use cards, waiting for opponent/timer- game manager can set back to action select 
    public enum PlayerState //
    {
        shipPlacement,
        placementReady,
        selectActions,
        selectActionLocation,
        haltPhase,
        waitPhase
    }
    public PlayerState status;
    // ship 1, ship 2, ship 3
    public List<Battleship> shipList = new List<Battleship>();

    public GameObject shipObj;

    // ships placed
    public int shipsPlaced = 0;
    // ships alive/healthy count

    public int shipsAlive;
    // current ship selected
    public Battleship shipSelected;
    // cards in view - will refer to the card objects.

    // message to user
    public Text message;
    public int player = 0;
    public bool lookingForPOS = false;
    ExitGames.Client.Photon.Hashtable playerProperty = new ExitGames.Client.Photon.Hashtable();
    // lockInButton- needs to be created
    // cancel choice button - needs to be created in order to revert back from selectActionLocation state to selectActions state without locking in a choice.

    // functions
    // implement touchControls
    private TouchControls touchControls;
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;
    private void Awake()
    {
        touchControls = new TouchControls();
    }
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
        Debug.Log("PlayerController script detected start touch: " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
        if (OnStartTouch != null) OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
    }
    private void EndTouch(InputAction.CallbackContext context)
    {
        Debug.Log("PlayerController script detected end touch:  " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
        if (OnEndTouch != null) OnEndTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
    }
    // createShip

    void createShip(Battleship ship)
    {
        // instantiate the ship, making it a child of the player, and assigning "ship" to its script, so that we can access its characteristics. 

        ship = Instantiate(shipObj, this.transform).GetComponent<Battleship>();
    }
    // handle ship placement
    void handleShipPlacement(string type)
    {
        // will be called on click during shipPlacement state
        
        //print(Mathf.RoundToInt(mouseWorldPos.x) + " " + Mathf.RoundToInt(mouseWorldPos.y));
        
        // if ship is already placed there, do nothing
        Tile tilePos = getTile(type);
        if (tilePos != null && tilePos.Occupied == false)
        {
            GameObject ship = Instantiate(shipObj, this.transform);
            ship.GetComponent<Battleship>().position = tilePos.transform.position;

            sendShipData(ship.GetComponent<Battleship>(), shipsPlaced); //should send the same ship to both parties

            shipsPlaced++;
            Destroy(ship);
        }

        // otherwise if(shipsPlaced = 0, createShip(ship1), if shipsplaced = 1, createShip(ship2), if shipsPlaced = 2, createShip(ship3)
        // increment shipsPlaced;
        if (shipsPlaced == 3)
        {
            ChangeState(PlayerState.placementReady);
            //PhotonView photonView = PhotonView.Get(this);
            //photonView.RPC("UpdatePlayerController", RpcTarget.All, this, player);
            GameManager.Instance.sendPC(player);
        }
        else
        {
            lookingForPOS=true;
        }
    }

    private Tile getTile(string type)
    {
        Vector2 v2;
        if(type == "mouse")
        {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        v2 = new Vector2(Mathf.RoundToInt(mouseWorldPos.x), Mathf.RoundToInt(mouseWorldPos.y));
        } else
        {
            v2 = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        }

        Tile tilePos = GridManager.instance.GetTilePOS(v2, player);
        return tilePos;
    }

    public Battleship getShip(string type)
    {
        Vector2 v2;
        if(type == "mouse")
        {
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            v2 = new Vector2(Mathf.RoundToInt(mouseWorldPos.x), Mathf.RoundToInt(mouseWorldPos.y));
        } else
        {
            v2 = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        }

        foreach (Battleship ship in shipList)
        {
            if (ship.position == v2)
            {
                return ship;
            }
        }

            return null;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ChangeState(PlayerState.shipPlacement);
        //lookingForPOS=true;
        touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    }

    // Update is called once per frame
    void Update()
    {
        //print("Local Player Actor Number " + PhotonNetwork.LocalPlayer.ActorNumber);
        if (lookingForPOS && (Input.GetMouseButtonDown(0)) && player == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            lookingForPOS=false;
            handleShipPlacement("mouse");
        }
        if (lookingForPOS && OnStartTouch != null  && player == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            lookingForPOS = false;
            handleShipPlacement("touch");
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

    public void sendShipData(Battleship bs, int index)
    {
        photonView.RPC("UpdateSingleShip", RpcTarget.All, bs.position, bs.target, bs.choice, index, player, bs.shield, bs.destroyed);
    }

    public int locateShipInList(Battleship bs)
    {
        for (int j = 0; j < shipList.Count; j++)
        {
            if (shipList[j].position == bs.position)
            {
                print("ship index: " + j);
                return j;
            }
        }
        print("ship index: -1");
        return -1;
    }

    public bool checkIfDefeated()
    {
        bool b = true;
        foreach (Battleship bs in shipList)
        {
            if(!bs.destroyed)
            {
                b = false;
                break;
            }
        }

        return b;
    }
}   

