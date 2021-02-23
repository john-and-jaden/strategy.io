using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Building : Interactable
{
    [SerializeField] private float ghostAlpha = 0.2f;
    [SerializeField] private float inProgressAlpha = 0.5f;
    [SerializeField] private bool preBuilt = false;

    public float BuildTime { get; set; }

    protected bool completed;
    public bool Completed { get { return completed; } }

    private Color spriteColor;

    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;

        if (!preBuilt) SetAlpha(ghostAlpha);
        health = preBuilt ? maxHealth : 0;
        completed = preBuilt;

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
