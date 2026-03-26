using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private Image timerImage;

    private void Start()
    {
        gameManager = GameManager.Instance;

        gameManager.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void Update()
    {
        if (gameManager.IsGamePlaying())
        {
            timerImage.fillAmount = gameManager.GetPlayingTimerNormalized();
        }
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (gameManager.IsGamePlaying())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
