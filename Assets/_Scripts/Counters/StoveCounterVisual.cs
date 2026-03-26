using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnVisual;
    [SerializeField] private GameObject fizzleParticle;

    private void Start()
    {
        stoveCounter.OnCook += StoveCounter_OnCook;
        stoveCounter.OffCook += StoveCounter_OffCook;
    }

    private void StoveCounter_OnCook(object sender, System.EventArgs e)
    {
        Debug.Log(sender);
        stoveOnVisual.SetActive(true);

        fizzleParticle.SetActive(true);
    }

    private void StoveCounter_OffCook(object sender, System.EventArgs e)
    {
        stoveOnVisual.SetActive(false);


        fizzleParticle.SetActive(false);
    }
}
