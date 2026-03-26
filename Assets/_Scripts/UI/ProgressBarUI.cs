using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;
    [SerializeField] private Image backgroundImage;

    private IHasProgress hasProgress;

    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();

        if (hasProgress == null)
        {
            Debug.LogError("Game Object: " + hasProgressGameObject + " does not have a component that implements IHasProgress!");
        }

        hasProgress.OnProgressChanged += HasProgess_OnProgressChanged;

        barImage.fillAmount = 0;

        Hide();
    }

    private void HasProgess_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if(e.progressNormalized == 1 || e.progressNormalized == 0)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        barImage.enabled = true;
        backgroundImage.enabled = true;
    }
    private void Hide()
    {
        barImage.fillAmount = 0;
        barImage.enabled = false;
        backgroundImage.enabled = false;
    }

    //private void CuttingCounter_OnProgressEnd(object sender, System.EventArgs e)
    //{
    //    barImage.fillAmount = 0;
    //    barImage.enabled = false;
    //    backgroundImage.enabled = false;
    //}

    //private void CuttingCounter_OnProgressStart(object sender, System.EventArgs e)
    //{
    //    barImage.enabled = true;
    //    backgroundImage.enabled = true;
    //}
}
