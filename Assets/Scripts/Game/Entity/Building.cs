using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Building : Interactable
{
    [SerializeField] private float ghostAlpha = 0.2f;
    [SerializeField] private float inProgressAlpha = 0.5f;

    protected bool completed;
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
        bool capped = base.GainHealth(gain);

        if (!completed)
        {
            if (capped)
            {
                SetAlpha(1f);
                completed = true;
            }
            else
            {
                SetAlpha(inProgressAlpha);
            }
        }

        return capped;
    }

    private void SetAlpha(float alpha)
    {
        spriteColor.a = alpha;
        spriteRenderer.color = spriteColor;
    }
}
