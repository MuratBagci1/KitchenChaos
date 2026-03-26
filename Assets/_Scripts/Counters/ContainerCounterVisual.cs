using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private ContainerCounter container;
    private const string OPEN_CLOSE = "OpenClose";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        container.OnPlayerGrabbedObject += PlayAnimation;
    }

    private void PlayAnimation(object sender, EventArgs e)
    {
        animator.SetTrigger(OPEN_CLOSE);
    }
}
