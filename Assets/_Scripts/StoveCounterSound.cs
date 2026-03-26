using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] StoveCounter stoveCounter;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnCook += StoveCounter_OnCook;
        stoveCounter.OffCook += StoveCounter_offCook;
    }

    private void StoveCounter_OnCook(object sender, System.EventArgs e)
    {
        audioSource.Play();
    }

    private void StoveCounter_offCook(object sender, System.EventArgs e)
    {
        audioSource.Stop();
    }
}
