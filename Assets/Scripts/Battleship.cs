using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleship : MonoBehaviour
{
    // Charactertistics of a ship
    // position(which square on the grid) need to check if this will be stored as a vector position, or a grid type position.

    // choice selected(shoot, scan, move, defend, unselected/default)
    public string choice;
    // destroyed or healthy
    public bool destroyed;
    //the target of its choice
    public double target;
    // shield = true or false
    public bool shield;
    // speed of movement(maybe unnecessary if we just want to make the ship render in a different square rather than sliding over.)
    public double speed;
    // playerNumber( which player it belongs to)- I think we can just refer to its parent, and when we instantiate the ship, we can make it a child of the player
    
    // Functions


    // Start is called before the first frame update
    void Start()
    {
        //position = 0;
        destroyed = false;
        shield = false;
        target = 0.0;
        speed = 1; // we can decide how fast the ships will move
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
