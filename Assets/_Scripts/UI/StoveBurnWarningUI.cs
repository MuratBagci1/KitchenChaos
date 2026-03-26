using System;
using UnityEngine;
using UnityEngine.UI;

public class StoveBurnWarningUI : MonoBehaviour
{
    [SerializeField] private Image warningImage;
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private float colorChangeSpeed;
    [SerializeField] private float burnShowProgressAmount;


    private Color firstColor;
    private Color secondColor;

    private bool isBurning;
    private bool hasPlayedSound;

    private void Awake()
    {
        firstColor = new Color(1, 1, 1, 0); //white with 0 alpha
        
        secondColor = new Color(1, 0, 0, 1);
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        stoveCounter.OffCook += StoveCounter_OffCook;

        Hide();
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        isBurning = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;

        if (isBurning)
        {
            Show();
        }
        else if (warningImage.isActiveAndEnabled)
        {
            Hide();
        }
    }

    private void Show()
    {
        warningImage.enabled = true;

        float lerpTime = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
        Color lerpColor = Color.Lerp(firstColor, secondColor, lerpTime);

        warningImage.color = lerpColor; 
        
        if (lerpTime >= 0.95f && !hasPlayedSound)
        {
            SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            hasPlayedSound = true;
        }
        else if(lerpTime < 0.1f)
        {
            hasPlayedSound = false;
        }
    }

    private void Hide()
    {
        warningImage.color = firstColor;
        warningImage.enabled = false;
    }

    private void StoveCounter_OffCook(object sender, System.EventArgs e)
    {
        Hide();
    }
}