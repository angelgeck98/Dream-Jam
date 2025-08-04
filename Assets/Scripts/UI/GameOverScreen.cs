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
    /*

    
    [SerializeField] private bool loopLastFrames = true;
    [SerializeField] private int loopFrameCount = 5;
    
    */
    //[SerializeField] private Animator deathAnimator;
    [SerializeField] private Image animationImage;
    [SerializeField] private Sprite[] animationFrames;
    [SerializeField] private float frameRate = 24f;

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

        if (animationImage!= null)
        {
            animationImage.enabled = false;
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
        animationImage.gameObject.SetActive(true);

        float frameDuration = 1f / frameRate;

        for (int i = 0; i < animationFrames.Length; i++)
        {
            animationImage.sprite = animationFrames[i];
            yield return new WaitForSecondsRealtime(frameDuration);
        }
    }
}