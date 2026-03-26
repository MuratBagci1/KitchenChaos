using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SroveBurnFlashingUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";

    private Animator animator;

    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private float flashTreshold;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        bool flashing = stoveCounter.IsFried() && e.progressNormalized >= flashTreshold;

        animator.SetBool(IS_FLASHING, flashing);
    }
}
