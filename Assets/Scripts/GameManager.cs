using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public RectTransform levelName;
    public RectTransform derailed;
    public GameObject fade;

    private Train[] trains;
    private Player player;
    private Selector selector;
    private Map map;

    bool hasDerailed = false;
    bool hasReachedEnd = false;

    private void Awake()
    {
        trains = GameObject.FindObjectsOfType<Train>();
        player = GameObject.FindObjectOfType<Player>();
        selector = GameObject.FindObjectOfType<Selector>();
        map = GameObject.FindObjectOfType<Map>();
    }

    private void Update()
    {
        if (!hasReachedEnd && !hasDerailed)
        {
            bool finished = true;
            bool derailed = false;

            for (int i = 0; i < trains.Length; i++)
            {
                if (!trains[i].hasReachedEnd) finished = false;
                if (trains[i].hasDerailed) derailed = true;
            }

            if (derailed) OnDerailed();
            else if (finished) OnGameFinished();
        }
    }

    private void OnDerailed()
    {
        DisablePlayer();
        hasDerailed = true;

        foreach (Train train in trains)
        {
            train.enabled = false;
        }

        LeanTween.moveX(Camera.main.gameObject, trains[0].transform.position.x, 0.2f);

        LeanTween.move(derailed, new Vector3(0, -100, 0), 1f)
            .setEase(LeanTweenType.easeOutCirc)
            .setOnComplete(() =>
            {
                LeanTween.value(fade, SetAlphaCallback, 0, 1, 1).setOnComplete(() =>
                {
                    SceneManager.LoadScene(map.levelName);
                });
            }
        );
    }
                

    private void OnGameFinished()
    {
        DisablePlayer();
        hasReachedEnd = true;

        LeanTween.value(fade, SetAlphaCallback, 0, 1, 1).setOnComplete(() =>
        {
            SceneManager.LoadScene(map.nextLevel);
        });
    }   

    private void DisablePlayer()
    {
        player.enabled = false;
        selector.gameObject.SetActive(false);
    }

    private void Start()
    {
        LeanTween.value(fade, SetAlphaCallback, 1, 0, 1).setOnComplete(() =>
        {
            LeanTween.move(levelName, new Vector3(0, -100, 0), 1f)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(() =>
            {
                LeanTween.move(levelName, new Vector3(0, 100, 0), 1f)
                    .setDelay(2f)
                    .setEase(LeanTweenType.easeInCirc).setOnComplete(() =>
                    {
                        Destroy(levelName.gameObject);
                    }
                );
            });
        });
    }

    private void SetAlphaCallback(float alpha)
    {
        fade.GetComponent<Image>().color = new Color(0, 0, 0, alpha);
    }
}
