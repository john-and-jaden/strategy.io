using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Building : Interactable
{
    [SerializeField] private float ghostAlpha = 0.2f;
    [SerializeField] private float inProgressAlpha = 0.5f;

    public float BuildTime { get; set; }

    protected bool completed;
    private Color spriteColor;

    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;

        health = 0;
        SetAlpha(ghostAlpha);

        base.Awake();
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
