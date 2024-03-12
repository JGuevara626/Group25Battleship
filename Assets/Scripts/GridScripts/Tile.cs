using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer sRender;
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject targeted;
    public bool Occupied = false;
    public bool stayHighlighted = false;
    public void switchColor(bool isOff)
    {
        sRender.color = isOff ? offsetColor : baseColor;
    }

    public void setUnit(Battleship unit)
    {
        if(unit.OccupiedTile != null)
        {
            unit.OccupiedTile.Occupied = false;
        }
        unit.transform.position = transform.position;
        unit.position = transform.position;
        Occupied = true;
        unit.OccupiedTile = this;
    }

    private void OnMouseEnter() 
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (!stayHighlighted)
        {
            highlight.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        //print((int)transform.position.x + " " + (int)transform.position.y);
    }

    public void SetHighlight(bool b)
    {
        highlight.SetActive(b);
        stayHighlighted = b;
    }

    public void SetTargeted(bool b)
    {
        targeted.SetActive(b);
    }
    public void SetDefeated()
    {
        sRender.color = Color.grey;
    }
}
