using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    [SerializeField] private int gridWidth, gridHeight;

    [SerializeField] private Tile tile;
    [SerializeField] private Transform cam;

    //private Dictionary<Vector2, Tile> tilesDic;
    private Dictionary<Vector2, Tile> RTilemap;
    private Dictionary<Vector2, Tile> LTilemap;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        //GenerateGrid();

        //var testSpawn = Instantiate(bs);
        //var v = new Vector2(2, 4);
        //var getTile = GetTilePOS(v, true);
        //getTile.setUnit(testSpawn);
    }

    public void GenerateGrid()
    {
        RTilemap = new Dictionary<Vector2, Tile>();
        LTilemap = new Dictionary<Vector2, Tile>();
        for (int i = 0; i < gridWidth; i++)
        {
            if (i == Mathf.RoundToInt(gridWidth / 2))
            {
                continue;
            }
            else
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    var instTile = Instantiate(tile, new Vector3(i, j), Quaternion.identity);
                    instTile.name = $"Tile {i} {j}";
                    instTile.transform.parent = transform;

                    if ((i + j) % 2 == 1)//((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                    {
                        instTile.switchColor(true);
                    }
                    else
                    {
                        instTile.switchColor(false);
                    }

                    if (i <= 7)
                    {
                        LTilemap[new Vector2(i, j)] = instTile;
                    }
                    else
                    {
                        RTilemap[new Vector2(i, j)] = instTile;
                    }
                }
            }
        }
        cam.transform.position = new Vector3((float)gridWidth / 2 - 0.5f, (float)gridHeight / 2 + 0.5f, -10);
    }

    public Tile GetTilePOS(Vector2 pos, int ifLeft)
    {
        switch (ifLeft){
            case 1:
                if (LTilemap.TryGetValue(pos, out var l))
                {
                    return l;
                }
                break;
            case 2:
                if (RTilemap.TryGetValue(pos, out var r))
                {
                    return r;
                }
                break;
        }
        return null;
    }

    public Tile GetTileAnyPOS(Vector2 pos)
    {
        if (LTilemap.TryGetValue(pos, out var l))
        {
            return l;
        }
        else if (RTilemap.TryGetValue(pos, out var r))
        {
            return r;
        }
        else { return null; }
    }

    public List<Tile> GetDirectionalTiles(Vector2 centerV, int b)
    {
        List<Tile> list = new List<Tile>();
        Vector2 upV = new Vector2(centerV.x, centerV.y + 1);
        Tile tileUp = GridManager.instance.GetTilePOS(upV, b);
        if (tileUp != null && !tileUp.Occupied) { list.Add(tileUp); }

        Vector2 downV = new Vector2(centerV.x, centerV.y - 1);
        Tile tileDown = GridManager.instance.GetTilePOS(downV, b);
        if (tileDown != null && !tileDown.Occupied) { list.Add(tileDown);}

        Vector2 leftV = new Vector2(centerV.x - 1, centerV.y);
        Tile tileLeft = GridManager.instance.GetTilePOS(leftV, b);
        if(tileLeft != null && !tileLeft.Occupied) { list.Add(tileLeft);}

        Vector2 rightV = new Vector2(centerV.x + 1, centerV.y);
        Tile tileRight = GridManager.instance.GetTilePOS(rightV, b);
        if(tileRight != null && !tileRight.Occupied) { list.Add(tileRight);}


        return list;
    }
}
