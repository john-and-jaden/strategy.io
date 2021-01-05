using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [System.Serializable] public class DeathEvent : UnityEvent { }
    [System.Serializable] public class HealthChangedEvent : UnityEvent<float> { }
    [System.Serializable] public class MaxHealthChangedEvent : UnityEvent<float> { }

    [SerializeField] private SpriteRenderer hoverIndicator;
    [SerializeField] private SpriteRenderer selectIndicator;

    [SerializeField] private float maxHealth = 10;
    public float MaxHealth { get { return maxHealth; } }

    private float health;
    public float Health { get { return health; } }

    private DeathEvent onDeath = new DeathEvent();
    private HealthChangedEvent onHealthChanged = new HealthChangedEvent();
    private MaxHealthChangedEvent onMaxHealthChanged = new MaxHealthChangedEvent();

    protected virtual void Awake()
    {
        health = maxHealth;
    }

    public void Hover()
    {
        if (hoverIndicator == null) return;
        hoverIndicator.enabled = true;
    }

    public void CancelHover()
    {
        if (hoverIndicator == null) return;
        hoverIndicator.enabled = false;
    }

    public void Select()
    {
        if (selectIndicator == null) return;
        selectIndicator.enabled = true;
    }

    public void CancelSelect()
    {
        if (selectIndicator == null) return;
        selectIndicator.enabled = false;
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

    public void AddDestroyedListener(UnityAction listener)
    {
        onDeath.AddListener(listener);
    }

    public void RemoveDestroyedListener(UnityAction listener)
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
}
