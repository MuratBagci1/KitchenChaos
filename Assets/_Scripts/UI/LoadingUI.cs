using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;

    private const string LOADING_TEXT = "LOADING";

    private float loadingTimer = .5f;
    private float loadingTimerMax = .5f;

    int dotCount = 0;

    private void Update()
    {
        loadingTimer -= Time.deltaTime;
        if (loadingTimer < 0)
        {
            loadingTimer = loadingTimerMax;

            dotCount++;
            dotCount = dotCount % 4;

            loadingText.text = LOADING_TEXT + new string('.', dotCount);
        }
    }
}
