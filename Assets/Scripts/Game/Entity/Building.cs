using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Building : Interactable
{
    [SerializeField] private float ghostAlpha = 0.2f;
    [SerializeField] private float inProgressAlpha = 0.5f;

    private Color spriteColor;

    private SpriteRenderer spriteRenderer;

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;

        health = 0;
        SetAlpha(ghostAlpha);
    }

    public override bool GainHealth(float gain)
    {
        bool completed = base.GainHealth(gain);
        if (completed) SetAlpha(1f);
        else SetAlpha(inProgressAlpha);
        return completed;
    }

    private void SetAlpha(float alpha)
    {
        spriteColor.a = alpha;
        spriteRenderer.color = spriteColor;
    }
}
