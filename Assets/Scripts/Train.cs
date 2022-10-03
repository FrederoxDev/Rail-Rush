using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Train : MonoBehaviour
{
    public Map map;
    public Dir currentDir;
    public int targetX;
    public int targetY;

    public int startX;
    public int startY;
    
    public SpriteRenderer sr;
    public Sprite[] sprites;

    public float timeBetweenTracks = 0.5f;
    float currentLerpTime = 0;
    public float delay = 2f;

    float boostTimer = 0;

    public bool hasReachedEnd = false;
    public bool hasDerailed = false;

    private void Awake()
    {
        currentLerpTime = -delay;
        sr = GetComponent<SpriteRenderer>();
        startX = map.entryPoint.x;
        startY = map.entryPoint.y;
        targetX = map.entryPoint.x;
        targetY = map.entryPoint.y;
        transform.position = new Vector3(startX, startY, -2);
        currentDir = map.trainDir;
    }

    private void Update()
    {
        boostTimer += Time.deltaTime;
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > timeBetweenTracks) currentLerpTime = timeBetweenTracks;

        if (boostTimer > 10)
        {
            timeBetweenTracks *= 0.85f;
            boostTimer = 0;
        }

        SetPosition(Vector3.Lerp(
            new Vector3(startX, startY, -2),
            new Vector3(targetX, targetY, -2),
            currentLerpTime / timeBetweenTracks
        ));

        if (currentLerpTime == timeBetweenTracks)
        {
            startX = targetX;
            startY = targetY;

            if (startX == map.exitPoint.x && startY == map.exitPoint.y)
            {
                hasReachedEnd = true;
                gameObject.SetActive(false);
                return;
            }

            SetTarget();
            currentLerpTime = 0;
        }
    }

    void SetPosition(Vector3 vec)
    {
        transform.position = new Vector3(vec.x, vec.y, -2);
    }

    void SetTarget()
    {
        if (currentDir == Dir.South)
        {
            if (map.IsRail(targetX, targetY - 1)) MoveS();
            else if (map.IsRail(targetX + 1, targetY)) MoveE();
            else if (map.IsRail(targetX - 1, targetY)) MoveW();
            else
            {
                hasDerailed = true;
            }
        }

        else if (currentDir == Dir.East)
        {
            if (map.IsRail(targetX + 1, targetY)) MoveE();
            else if (map.IsRail(targetX, targetY + 1)) MoveN();
            else if (map.IsRail(targetX, targetY - 1)) MoveS();
            else
            {
                hasDerailed = true;
            }
        }

        else if (currentDir == Dir.North)
        {
            if (map.IsRail(targetX, targetY + 1)) MoveN();
            else if (map.IsRail(targetX + 1, targetY)) MoveE();
            else if (map.IsRail(targetX - 1, targetY)) MoveW();
            else
            {
                hasDerailed = true;
            }
        }

        else if (currentDir == Dir.West)
        {
            if (map.IsRail(targetX - 1, targetY)) MoveW();
            else if (map.IsRail(targetX, targetY + 1)) MoveN();
            else if (map.IsRail(targetX, targetY - 1)) MoveS();
            else
            {
                hasDerailed = true;
            }
        }
    }

    void MoveN()
    {
        currentDir = Dir.North;
        targetY += 1;
        sr.sprite = sprites[0];
        sr.flipX = false;
    }

    void MoveS()
    {
        currentDir = Dir.South;
        targetY -= 1;
        sr.sprite = sprites[1];
        sr.flipX = false;
    }

    void MoveE()
    {
        currentDir = Dir.East;
        targetX += 1;
        sr.sprite = sprites[2];
        sr.flipX = false;

    }

    void MoveW()
    {
        currentDir = Dir.West;
        targetX -= 1;
        sr.sprite = sprites[2];
        sr.flipX = true;
    }
}

public enum Dir
{
    North,
    East,
    South,
    West
}
