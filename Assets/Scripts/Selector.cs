using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    public Map map;
    public Player player;
    public float timer = 0;
    public Slider slider;
    private AudioSource audioSource;
    public AudioClip clip;

    private void Awake()
    {
        timer = 0;
        slider.transform.position = new Vector3(0, 0, -100);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosScreen.x, mousePosScreen.y, Camera.main.nearClipPlane));
        Vector3 point = (mousePos - player.transform.position).normalized;

        int compassDir = Mathf.FloorToInt((Mathf.Round(Mathf.Atan2(point.y, point.x) / (2 * Mathf.PI / 8)) + 8) % 8);

        if (compassDir == 0) point = new Vector2(1, 0);
        else if (compassDir == 1) point = new Vector2(1, 1);
        else if (compassDir == 2) point = new Vector2(0, 1);
        else if (compassDir == 3) point = new Vector2(-1, 1);
        else if (compassDir == 4) point = new Vector2(-1, 0);
        else if (compassDir == 5) point = new Vector2(-1, -1);
        else if (compassDir == 6) point = new Vector2(0, -1);
        else if (compassDir == 7) point = new Vector2(1, -1);

        point += player.transform.position;
        transform.position = new Vector3(Mathf.Floor(point.x + 0.5f), Mathf.Floor(point.y + 0.5f), -2);

        if (Input.GetMouseButtonDown(0))
        {
            int x = Mathf.FloorToInt(point.x + 0.5f);
            int y = Mathf.FloorToInt(point.y + 0.5f);
            if (x > map.mapWidth || y > map.mapHeight || x < 0 || y < 0) return;
            TileData tileData = map.tiles[x, y];

            // Drop Items
            if (player.item != Items.None && tileData.item == Items.None && tileData.tile == Tiles.Grass)
            {
                map.Place(new TileData(Tiles.Grass, player.item), x, y);
                player.SetItem(Items.None);
            }

            // Pickup items
            else if (tileData.item != Items.None && player.item == Items.None)
            {
                map.Place(new TileData(Tiles.Grass, Items.None), x, y);
                player.SetItem(tileData.item);
            }

            else if (tileData.item == Items.Wood && player.item == Items.Rock)
            {
                map.Place(new TileData(Tiles.Rail, Items.None), x, y);
                player.SetItem(Items.None);
            }

            else if (tileData.item == Items.Rock && player.item == Items.Wood)
            {
                map.Place(new TileData(Tiles.Rail, Items.None), x, y);
                player.SetItem(Items.None);
            }
        }

        if (Input.GetMouseButton(0))
        {
            int x = Mathf.FloorToInt(point.x + 0.5f);
            int y = Mathf.FloorToInt(point.y + 0.5f);

            if (x >= map.mapWidth || y >= map.mapHeight || x < 0 || y < 0) return;

            TileData tileData = map.tiles[x, y];

            if (tileData.tile == Tiles.Tree || tileData.tile == Tiles.Rock)
            {
                timer += Time.deltaTime;
                slider.transform.position = Camera.main.WorldToScreenPoint(new Vector3(x, y, -3));
                slider.value = 1 - (timer / 0.5f);
            }

            // Destroy trees
            if (tileData.tile == Tiles.Tree && player.item == Items.None && timer > 0.5f)
            {
                map.Place(new TileData(Tiles.Grass, Items.Wood), x, y);
                slider.transform.position = new Vector3(0, 0, -100);
                timer = 0;
                audioSource.PlayOneShot(clip, 0.5f);
            }

            // Destroy Rocks
            else if (tileData.tile == Tiles.Rock && player.item == Items.None && timer > 0.5f)
            {
                map.Place(new TileData(Tiles.Grass, Items.Rock), x, y);
                slider.transform.position = new Vector3(0, 0, -100);
                timer = 0;
                audioSource.PlayOneShot(clip, 0.5f);
            }
        }
    
        if (Input.GetMouseButtonUp(0))
        {
            timer = 0;
            slider.transform.position = new Vector3(0, 0, -100);
        }
    }
}
