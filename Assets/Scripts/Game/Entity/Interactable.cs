using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    private const float NON_INTERACTIVE_PERIOD = 0.1f;

    [System.Serializable] public class DeathEvent : UnityEvent { }
    [System.Serializable] public class HealthChangedEvent : UnityEvent<float> { }
    [System.Serializable] public class MaxHealthChangedEvent : UnityEvent<float> { }

    [SerializeField] private SpriteRenderer hoverIndicator;
    [SerializeField] private SpriteRenderer selectIndicator;

    [SerializeField] private float maxHealth = 10;
    public float MaxHealth { get { return maxHealth; } }

    private float health;
    public float Health { get { return health; } }

    protected bool interactive = false;

    private DeathEvent onDeath = new DeathEvent();
    private HealthChangedEvent onHealthChanged = new HealthChangedEvent();
    private MaxHealthChangedEvent onMaxHealthChanged = new MaxHealthChangedEvent();

    protected virtual void Awake()
    {
        health = maxHealth;
    }

    protected virtual void Start()
    {
        StartCoroutine(DelayInteractivity());
    }

    public virtual void Hover()
    {
        if (!interactive) return;
        if (hoverIndicator != null) hoverIndicator.enabled = true;
    }

    public virtual void Unhover()
    {
        if (!interactive) return;
        if (hoverIndicator != null) hoverIndicator.enabled = false;
    }

    public virtual void Select()
    {
        if (!interactive) return;
        if (selectIndicator != null) selectIndicator.enabled = true;
    }

    public virtual void Deselect()
    {
        if (!interactive) return;
        if (selectIndicator != null) selectIndicator.enabled = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        onHealthChanged.Invoke(health);
        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void AddDeathListener(UnityAction listener)
    {
        onDeath.AddListener(listener);
    }

    public void RemoveDeathListener(UnityAction listener)
    {
        onDeath.RemoveListener(listener);
    }

    public void AddHealthChangedListener(UnityAction<float> listener)
    {
        onHealthChanged.AddListener(listener);
    }

    public void RemoveHealthChangedListener(UnityAction<float> listener)
    {
        onHealthChanged.RemoveListener(listener);
    }

    public void AddMaxHealthChangedListener(UnityAction<float> listener)
    {
        onMaxHealthChanged.AddListener(listener);
    }

    public void RemoveMaxHealthChangedListener(UnityAction<float> listener)
    {
        onMaxHealthChanged.RemoveListener(listener);
    }

    private IEnumerator DelayInteractivity()
    {
        interactive = false;
        yield return new WaitForSeconds(NON_INTERACTIVE_PERIOD);
        interactive = true;
    }
}
