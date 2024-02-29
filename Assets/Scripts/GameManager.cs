using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //characteristics of game manager

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

    //functions

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
    void HandleShipAction(string action, double target)
    {
        // if ship is not alive, just skip 
        // if ship action is shoot, shootCannon(pl)
        // if ship action is move, moveShip()
        // if ship action is defend, activate shield
        // if ship action is default, shoot a cannonball at a random square?

        // wipe ship's selection clean
    }


    //shootCannon(shotFrom, shotTo)
    void shootCannon()
    {
        // spawn a cannonball at shootFrom and make its target shotTo
    }
    //moveShip(moveTo)'
    void moveShip()// figure out what data type a ship's position is
    {
        // position = newPosition;
    }
    //
    void Start()
    {
        state = gameState.shipPlacement;
    }

    // Update is called once per frame
    void Update()
    {
        // if timer expires or both players are finished locking in their choices, then change states to the battleAnimation state.
        if (timer.timer == 0f || (player1.status == PlayerController.PlayerState.waitPhase && player2.status == PlayerController.PlayerState.waitPhase))
        { 
            HandleTurnOrder();
        }

    }
}
