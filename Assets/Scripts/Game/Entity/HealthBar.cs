using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Interactable parent;
    [SerializeField] private SpriteRenderer fillBar;
    [SerializeField] private SpriteRenderer backgroundBar;
    [SerializeField] private float fadeDelay = 2f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private bool alwaysShowIfDamaged = false;

    private float health;
    private float maxHealth;
    private bool forceDisplay;
    private float alpha;
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

        parent.AddSelectedListener(ActivateForcemode);
        parent.AddDeselectedListener(DeactivateForcemode);
        parent.AddHealthChangedListener(UpdateHealth);
        parent.AddMaxHealthChangedListener(UpdateMaxHealth);

        fillBarColor = fillBar.color;
        maxFillAlpha = fillBar.color.a;
        backgroundBarColor = backgroundBar.color;
        maxBackgroundAlpha = backgroundBar.color.a;
    }

    void Start()
    {
        health = parent.Health;
        maxHealth = parent.MaxHealth;
        alpha = 0f;
        UpdateDisplay();
    }

    private void ActivateForcemode()
    {
        forceDisplay = health < maxHealth;
        UpdateDisplay();
    }

    private void DeactivateForcemode()
    {
        forceDisplay = false;
        UpdateDisplay();
    }

    private void UpdateHealth(float health)
    {
        this.health = health;
        alpha = 1f;
        UpdateDisplay();
        StartFadeTransition();
    }

    private void UpdateMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        alpha = 1f;
        UpdateDisplay();
        StartFadeTransition();
    }

    private void UpdateDisplay()
    {
        bool showDamaged = alwaysShowIfDamaged && health < maxHealth;
        float a = forceDisplay || showDamaged ? 1f : alpha;
        fillBar.size = new Vector2(health / maxHealth, 1);
        fillBarColor.a = maxFillAlpha * a;
        fillBar.color = fillBarColor;
        backgroundBarColor.a = maxBackgroundAlpha * a;
        backgroundBar.color = backgroundBarColor;
    }

    private void StartFadeTransition()
    {
        if (fadeTransition != null) StopCoroutine(fadeTransition);
        if (delayedFade != null) StopCoroutine(delayedFade);
        delayedFade = StartCoroutine(DelayedFade());
    }

    private IEnumerator DelayedFade()
    {
        alpha = 1f;
        UpdateDisplay();
        yield return fadeDelayInstruction;
        fadeTransition = StartCoroutine(FadeTransition());
    }

    private IEnumerator FadeTransition()
    {
        float fadeTimer = 0;
        while (fadeTimer <= fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            
            alpha = Mathf.Lerp(1f, 0, fadeTimer / fadeDuration);
            UpdateDisplay();

            yield return null;
        }
    }
}
