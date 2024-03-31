using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{

    // Characteristics of a cannon ball

    // ship position to spawn from
    // target position
    // speed
    public Vector3 position;
    public Vector3 target;
    public bool launched;
    float distance;
    float remainingDistance;
    public int isLeft;
    // Functions

    // handler for when it gets to target position
    void cannonBallArrival()
    {
        // if cannonball gets there, and there is no ship, do nothing?
        // if cannonball gets there and there is a ship with a shield, pop the shield, and display "the cannonball was deflected!"
        // if cannonball gets there and it hits a ship with no shield, make the ship sink and display "Bullseye! A ship was destroyed! Way to go Hotshot!" 
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (launched)
        {
            //transform.position = Vector3.Lerp(position, target, Time.deltaTime);
            //if (transform.position == target)
            //{
            //    //call game manager
            //    //Destroy(gameObject);
            //}
            transform.position = Vector3.Lerp(position, target, 1 - (remainingDistance / distance));
            remainingDistance -= 18f * Time.deltaTime;

            if (remainingDistance <= 0)
            {
                launched = false;
                if(isLeft == 1)
                {
                    Tile t = GridManager.instance.GetTilePOS(target, isLeft);
                    t.waterhit();
                }
                else
                {
                    Tile t = GridManager.instance.GetTilePOS(target, isLeft);
                    t.waterhit();
                }
                //Tile t = GridManager.instance.GetTilePOS(target, isLeft);
                //t.waterhit();
                Destroy(gameObject);
            }
        }
    }

    public void shotBall(Vector2 pv2, Vector2 tv2, int p)
    {
        transform.position = pv2;
        isLeft = p;
        position = pv2;
        target = tv2;
        launched = true;
        distance = Vector3.Distance(position, tv2);
        remainingDistance = distance;
    }
}
