using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject fade;

    private void Start()
    {
        LeanTween.value(fade, SetAlphaCallback, 1, 0, 1).setOnComplete(() =>
        {
            fade.SetActive(false);
        });
    }

    public void StartButtonPressed()
    {
        fade.SetActive(true);
        LeanTween.value(fade, SetAlphaCallback, 0, 1, 1).setOnComplete(() =>
        {
            SceneManager.LoadScene("level01");
        });
    }

    private void SetAlphaCallback(float alpha)
    {
        fade.GetComponent<Image>().color = new Color(0, 0, 0, alpha);
    }
}
