using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [Header("Game Over Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Image blackOverlay;

    [Header("Death Animation")]
    [SerializeField] private Image animationImage;
    [SerializeField] private float frameRate = 24f;
    [SerializeField] private bool loopLastFrames = true;
    [SerializeField] private int loopFrameCount = 5;
    [SerializeField] private Sprite[] animationFrames;
    private int index;
    Coroutine coroutineAnim;

    void Start()
    {
        InitializeScreen();
    }

    private void InitializeScreen()
    {
        gameOverPanel?.SetActive(false);

        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);
            blackOverlay.color = new Color(0, 0, 0, 0f); // transparent black
        }

        if (animationImage != null)
        {
            animationImage.gameObject.SetActive(false);
        }
    }


    public void ShowGameOver()
    {
        StartCoroutine(FadeToBlack());
        StartCoroutine(PlayFrameAnimation());
    }

    private IEnumerator FadeToBlack()
    {
        if (blackOverlay == null) yield break;

        blackOverlay.color = Color.black;
            yield return null;
    }

    private IEnumerator PlayFrameAnimation()
    {

        yield return new WaitForSeconds(0.2f);
        if (index >= animationFrames.Length)
        {
            index = 0;
        }
        animationImage.sprite = animationFrames[index];
        index++;
        coroutineAnim = StartCoroutine(PlayFrameAnimation());
    }



}