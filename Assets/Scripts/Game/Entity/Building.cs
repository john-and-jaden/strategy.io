using UnityEngine;

public class Building : Interactable
{
    [SerializeField] private float startAlpha = 0.3f;

    private Color spriteColor;

    private SpriteRenderer spriteRenderer;

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;

        health = 0;
        SetAlpha(startAlpha);
    }

    public override bool GainHealth(float gain)
    {
        SetAlpha(Mathf.Lerp(startAlpha, 1f, health / maxHealth));
        return base.GainHealth(gain);
    }

    private void SetAlpha(float alpha)
    {
        spriteColor.a = alpha;
        spriteRenderer.color = spriteColor;
    }
}
