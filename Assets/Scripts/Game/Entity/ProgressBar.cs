using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fillBar;
    [SerializeField] private SpriteRenderer backgroundBar;
    [SerializeField] private float fadeDuration = 1f;

    private Color fillBarColor;
    private float maxFillAlpha;
    private Color backgroundBarColor;
    private float maxBackgroundAlpha;

    private Coroutine fadeTransition;

    private void Awake()
    {
        fillBarColor = fillBar.color;
        maxFillAlpha = fillBar.color.a;
        backgroundBarColor = backgroundBar.color;
        maxBackgroundAlpha = backgroundBar.color.a;

        UpdateDisplay(0);
    }

    public void HandleProgressChanged(float progress)
    {
        if (fadeTransition != null) StopCoroutine(fadeTransition);
        UpdateDisplay(1f);
        fillBar.size = new Vector2(progress, 1);
        if (progress >= 1) fadeTransition = StartCoroutine(FadeTransition());
    }

    private void UpdateDisplay(float alpha)
    {
        fillBarColor.a = maxFillAlpha * alpha;
        fillBar.color = fillBarColor;
        backgroundBarColor.a = maxBackgroundAlpha * alpha;
        backgroundBar.color = backgroundBarColor;
    }

    private IEnumerator FadeTransition()
    {
        float fadeTimer = 0;
        while (fadeTimer <= fadeDuration)
        {
            fadeTimer += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0, fadeTimer / fadeDuration);
            UpdateDisplay(alpha);

            yield return null;
        }
    }
}
