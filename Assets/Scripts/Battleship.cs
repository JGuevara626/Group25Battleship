using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleship : MonoBehaviour
{
    // Charactertistics of a ship
    //public string user;
    // position(which square on the grid) need to check if this will be stored as a vector position, or a grid type position.
    public Vector2 position;
    // choice selected(shoot, scan, move, defend, unselected/default)
    public string choice;
    // destroyed or healthy
    public bool destroyed;
    //the target of its choice
    public Vector2 target;
    // shield = true or false
    public bool shield;
    // speed of movement(maybe unnecessary if we just want to make the ship render in a different square rather than sliding over.)
    public Tile OccupiedTile;
    public int player = 0;
    private bool moving = false;
    private bool firing = false;
    private List<Tile> tileMovement = new List<Tile>();
    public bool lockChoice = false;
    [Space(10)]
    public GameObject lockHighlight;
    public GameObject Cannonball;
    public GameObject Focused;
    public ParticleSystem smoke;
    public ParticleSystem burst;
    public List<ParticleSystem> defenceParticles = new List<ParticleSystem>();
    // playerNumber( which player it belongs to)- I think we can just refer to its parent, and when we instantiate the ship, we can make it a child of the player
    private AudioSource audioCS;
    [Space(10)]
    public AudioClip explosionNoise;
    public AudioClip scanNoise;
    public AudioClip defendingNoise;
    // Functions
    // Start is called before the first frame update
    void Start()
    {
        audioCS = GameObject.FindWithTag("audioclipS").GetComponent<AudioSource>();
        //position = 0;
        destroyed = false;
        shield = false;
        //target = new Vector2(;
        //speed = 1; // we can decide how fast the ships will move
        choice = "unselected";
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving && Input.GetMouseButtonDown(0))
        {
            moving = false;
            setNewPOS();
        }

        if(firing && Input.GetMouseButtonDown(0))
        { 
            firing = false; 
            setNewTarget();
        }

        lockHighlight.SetActive(lockChoice);
    }

    public void shoot()
    {
        GameObject ball = Instantiate(Cannonball);
        if(player == 1)
        {
            ball.GetComponent<CannonBall>().shotBall(position, target, 2);
        }
        else
        {
            ball.GetComponent<CannonBall>().shotBall(position, target, 1);
        }

        if(PhotonNetwork.LocalPlayer.ActorNumber != player)
        {
            ball.GetComponent<Renderer>().enabled = false;
        }
    }

    public void setNewTarget()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 v2 = new Vector3(Mathf.RoundToInt(mouseWorldPos.x), Mathf.RoundToInt(mouseWorldPos.y));
        Tile t = GameManager.Instance.getEnemyTileSection(v2);

        if (t != null)
        {
            print("targeting: " + t);
            t.SetTargeted(true);
            target = v2;
            lockChoice = true;
            CardsGroupHandler.instance.handleCards();
            GameManager.Instance.shipToController(this);
            OccupiedTile.SetHighlight(false);
        }
        else
        {
            firing = true;
        }


    }

    public void startDefenceAnim()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == player)
        {
            GameObject.FindWithTag("audioclipS").GetComponent<AudioSource>().PlayOneShot(defendingNoise);
            StartCoroutine(playDefenceShots());
        }
    }

    IEnumerator playDefenceShots()
    {
        foreach(ParticleSystem dps in defenceParticles)
        {
            dps.Play();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void setNewPOS()
    {
        bool b = false;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 v2 = new Vector3(Mathf.RoundToInt(mouseWorldPos.x), Mathf.RoundToInt(mouseWorldPos.y));
        foreach (Tile tile in tileMovement)
        {
            if (tile.transform.position == v2)
            {
                target = v2;
                b = true;
                lockChoice = true;
                CardsGroupHandler.instance.handleCards();
                GameManager.Instance.shipToController(this);
                break;
            }
        }

        if (!b)
        {
            moving = true;
        }
        else
        {
            foreach (Tile tile in tileMovement)
            {
                tile.SetHighlight(false);
            }

            tileMovement.Clear();
            moving = false;
        }
    }

    public void setChoice(string s)
    {
        if (!lockChoice)
        {
            choice = s;
            switch (choice)
            {
                case "Move":
                    highlightSpaces();
                    break;
                case "Radar":
                    lockChoice = true;
                    CardsGroupHandler.instance.handleCards();
                    GameManager.Instance.shipToController(this);
                    break;
                case "Shield":
                    lockChoice = true;
                    shield = true;
                    CardsGroupHandler.instance.handleCards();
                    GameManager.Instance.shipToController(this);
                    break;
                case "Fire":
                    OccupiedTile.SetHighlight(true);
                    firing = true;
                    break;
            }
        }
    }

    public void resetChoice()
    {
        target = new Vector2(0, 0);
        choice = "unselected";
        lockChoice = false;
        shield = false;
    }

    private void highlightSpaces()
    {
        if (moving)
        {
            foreach (Tile tile in tileMovement)
            {
                tile.SetHighlight(false);
            }

            tileMovement.Clear();
            moving = false;

        }
        else
        {
            Vector2 centerV = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

            tileMovement = GridManager.instance.GetDirectionalTiles(centerV, player);
            foreach (Tile tile in tileMovement)
            {
                tile.SetHighlight(true);
            }
            moving = true;
        }
    }

    public void flipFocus()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == player)
        {
            Focused.SetActive(!Focused.activeSelf);
        }
    }

    public void killSelf()
    {
        choice = "unselected";
        lockChoice = false;
        burst.Play();
        smoke.Play();
        destroyed = true;
        audioCS.PlayOneShot(explosionNoise);
        OccupiedTile.SetDefeated();
        //GameManager.Instance.shipToController(this);
    }

}