using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    [Header("Map Data")]
    public string levelName;
    public string nextLevel;
    public int mapWidth;
    public int mapHeight;
    public Vector2Int entryPoint;
    public Vector2Int exitPoint;
    public Dir trainDir;
    public float minCamX = 0;
    public float maxCamX = 10;

    [Header("Tiles")]
    string map;
    public GameObject tilePrefab;
    public Sprite unknownSprite;
    public Sprite[] grassTiles;
    public Sprite[] waterTiles;
    public Sprite[] wallTiles;
    public Sprite[] railTiles;
    public Sprite[] railBackgrounds;
    public Sprite[] trees;
    public Sprite[] rocks;
    public Sprite[] items;
    public TileData[,] tiles;

    private void Awake()
    {
        tiles = new TileData[mapWidth, mapHeight];
        map = Resources.Load<TextAsset>(levelName).text.Replace("\n", "").Replace("\r", "");

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                string tileChar = map[(y * mapWidth) + x].ToString();

                if (tileChar == ".") tiles[x, y] = new TileData(Tiles.Grass, Items.None);
                else if (tileChar == "-") tiles[x, y] = new TileData(Tiles.Water, Items.None);
                else if (tileChar == "#") tiles[x, y] = new TileData(Tiles.Rail, Items.None);
                else if (tileChar == "^") tiles[x, y] = new TileData(Tiles.Tree, Items.None);
                else if (tileChar == "o") tiles[x, y] = new TileData(Tiles.Rock, Items.None);
                else if (tileChar == "=") tiles[x, y] = new TileData(Tiles.Wall, Items.None);
            }
        }

        DrawMap();
    }

    void DrawMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Random.InitState((y * mapWidth) + x);

                Sprite bgSprite = unknownSprite;
                Sprite fgSprite = null;

                Tiles tile = tiles[x, y].tile;
                Items item = tiles[x, y].item;

                if (tile == Tiles.Grass) bgSprite = grassTiles[Random.Range(0, grassTiles.Length - 1)];
                else if (tile == Tiles.Water) bgSprite = waterTiles[Random.Range(0, waterTiles.Length - 1)];
                else if (tile == Tiles.Wall) bgSprite = wallTiles[Random.Range(0, wallTiles.Length - 1)];
                else if (tile == Tiles.Tree)
                {
                    bgSprite = grassTiles[0];
                    fgSprite = trees[Random.Range(0, trees.Length - 1)];
                }
                else if (tile == Tiles.Rock)
                {
                    bgSprite = grassTiles[0];
                    fgSprite = rocks[Random.Range(0, rocks.Length - 1)];
                }
                else if (tile == Tiles.Rail)
                {
                    // Call range so the order of noise calls stays in order
                    // Keeping grass in right orientation
                    Random.Range(0, 1);
                    bool north = IsRail(x, y + 1);
                    bool east = IsRail(x + 1, y);
                    bool south = IsRail(x, y - 1);
                    bool west = IsRail(x - 1, y);

                    if (north && east && south && west)
                    {
                        fgSprite = railTiles[6];
                        bgSprite = railBackgrounds[6];
                    }
                    else if (north && south)
                    {
                        fgSprite = railTiles[0];
                        bgSprite = railBackgrounds[0];
                    }
                    else if (east && west)
                    {
                        fgSprite = railTiles[1];
                        bgSprite = railBackgrounds[1];
                    }
                    else if (south && east)
                    {
                        fgSprite = railTiles[2];
                        bgSprite = railBackgrounds[2];
                    }
                    else if (west && south)
                    {
                        fgSprite = railTiles[3];
                        bgSprite = railBackgrounds[3];
                    }
                    else if (north && east)
                    {
                        fgSprite = railTiles[4];
                        bgSprite = railBackgrounds[4];
                    }
                    else if (north && west)
                    {
                        fgSprite = railTiles[5];
                        bgSprite = railBackgrounds[5];
                    }
                    else if (north)
                    {
                        fgSprite = railTiles[0];
                        bgSprite = railBackgrounds[0];
                    }
                    else if (east)
                    {
                        fgSprite = railTiles[1];
                        bgSprite = railBackgrounds[1];
                    }
                    else if (south)
                    {
                        fgSprite = railTiles[0];
                        bgSprite = railBackgrounds[0];
                    }
                    else if (west)
                    {
                        fgSprite = railTiles[1];
                        bgSprite = railBackgrounds[1];
                    }
                    else
                    {
                        fgSprite = railTiles[1];
                        bgSprite = railBackgrounds[1];
                    }
                }

                if (item != Items.None)
                {
                    fgSprite = items[(int)item - 1];
                }

                int layer = tile == Tiles.Wall ? -3 : 1;

                GameObject bgTileObj = GameObject.Instantiate(tilePrefab, new Vector3(x, y, layer), Quaternion.identity);
                bgTileObj.GetComponent<SpriteRenderer>().sprite = bgSprite;
                bgTileObj.transform.parent = transform;
                bgTileObj.GetComponent<BoxCollider2D>().enabled = (tile == Tiles.Water || tile == Tiles.Tree || tile == Tiles.Rock || tile == Tiles.Wall);

                GameObject fgTileObj = GameObject.Instantiate(tilePrefab, new Vector3(x, y, -1), Quaternion.identity);
                fgTileObj.GetComponent<SpriteRenderer>().sprite = fgSprite;
                fgTileObj.transform.parent = transform;
                fgTileObj.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public bool IsRail(int x, int y)
    {
        if (x == entryPoint.x && y == entryPoint.y) return true;
        if (x == exitPoint.x && y == exitPoint.y) return true;

        if (x < 0 || y < 0) return false;
        if (x > mapWidth - 1 || y > mapHeight - 1) return false;

        try
        {
            if (tiles[x, y].tile == Tiles.Rail) return true;
        }

        catch
        {
            print($"({x}, {y})");
        }
        
        return false;
    }

    public void Place(TileData tile, int x, int y)
    {
        tiles[x, y] = tile;
        DrawMap();
    }


}

public class TileData
{
    public Tiles tile;
    public Items item;

    public TileData(Tiles tile, Items item)
    {
        this.tile = tile;
        this.item = item;
    }
}

public enum Tiles
{
    Grass,
    Water,
    Rail,
    Tree,
    Rock,
    Wall
}

public enum Items
{
    None,
    Wood,
    Rock
}