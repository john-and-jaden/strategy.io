using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Interactable parent;
    [SerializeField] private SpriteRenderer fillBar;
    [SerializeField] private SpriteRenderer backgroundBar;
    [SerializeField] private float fadeDelay;
    [SerializeField] private float fadeDuration;

    private float health;
    private float maxHealth;
    private Color fillBarColor;
    private float maxFillAlpha;
    private Color backgroundBarColor;
    private float maxBackgroundAlpha;

    private YieldInstruction fadeDelayInstruction;
    private Coroutine delayedFade;
    private Coroutine fadeTransition;

    void Awake()
    {
        fadeDelayInstruction = new WaitForSeconds(fadeDelay);

        parent.AddHealthChangedListener(UpdateHealth);
        parent.AddMaxHealthChangedListener(UpdateMaxHealth);
    }

    void Start()
    {
        health = parent.Health;
        maxHealth = parent.MaxHealth;

        fillBarColor = fillBar.color;
        maxFillAlpha = fillBar.color.a;
        backgroundBarColor = backgroundBar.color;
        maxBackgroundAlpha = backgroundBar.color.a;
        SetAlpha(0);
    }

    private void UpdateHealth(float health)
    {
        this.health = health;
        UpdateDisplay();
    }

    private void UpdateMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (fadeTransition != null) StopCoroutine(fadeTransition);
        SetAlpha(1);
        fillBar.size = new Vector2(health / maxHealth, 1);

        if (delayedFade != null) StopCoroutine(delayedFade);
        delayedFade = StartCoroutine(DelayedFade());
    }

    private IEnumerator DelayedFade()
    {
        yield return fadeDelayInstruction;
        fadeTransition = StartCoroutine(FadeTransition());
    }

    private IEnumerator FadeTransition()
    {
        float fadeTimer = 0;
        while (fadeTimer <= fadeDuration)
        {
            SetAlpha(Mathf.Lerp(1f, 0, fadeTimer / fadeDuration));

            fadeTimer += Time.deltaTime;
            yield return null;
        }
    }

    private void SetAlpha(float alpha)
    {
        fillBarColor.a = maxFillAlpha * alpha;
        fillBar.color = fillBarColor;
        backgroundBarColor.a = maxBackgroundAlpha * alpha;
        backgroundBar.color = backgroundBarColor;
    }
}
