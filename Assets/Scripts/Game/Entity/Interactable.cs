using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [System.Serializable] public class SelectedEvent : UnityEvent { }
    [System.Serializable] public class DeselectedEvent : UnityEvent { }
    [System.Serializable] public class HoveredEvent : UnityEvent { }
    [System.Serializable] public class UnhoveredEvent : UnityEvent { }
    [System.Serializable] public class DeathEvent : UnityEvent { }
    [System.Serializable] public class HealthChangedEvent : UnityEvent<float> { }
    [System.Serializable] public class MaxHealthChangedEvent : UnityEvent<float> { }

    [SerializeField] private SpriteRenderer hoverIndicator;
    [SerializeField] private SpriteRenderer selectIndicator;

    [SerializeField] private float maxHealth = 10;
    public float MaxHealth { get { return maxHealth; } }

    private float health;
    public float Health { get { return health; } }

    private SelectedEvent onSelected = new SelectedEvent();
    private DeselectedEvent onDeselected = new DeselectedEvent();
    private HoveredEvent onHovered = new HoveredEvent();
    private UnhoveredEvent onUnhovered = new UnhoveredEvent();
    private DeathEvent onDeath = new DeathEvent();
    private HealthChangedEvent onHealthChanged = new HealthChangedEvent();
    private MaxHealthChangedEvent onMaxHealthChanged = new MaxHealthChangedEvent();

    protected virtual void Awake()
    {
        health = maxHealth;
    }

    public virtual void Hover()
    {
        onHovered.Invoke();
        if (hoverIndicator != null) hoverIndicator.enabled = true;
    }

    public virtual void Unhover()
    {
        onUnhovered.Invoke();
        if (hoverIndicator != null) hoverIndicator.enabled = false;
    }

    public virtual void Select()
    {
        onSelected.Invoke();
        if (selectIndicator != null) selectIndicator.enabled = true;
    }

    public virtual void Deselect()
    {
        onDeselected.Invoke();
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

    public void AddSelectedListener(UnityAction listener)
    {
        onSelected.AddListener(listener);
    }

    public void RemoveSelectedListener(UnityAction listener)
    {
        onSelected.RemoveListener(listener);
    }

    public void AddDeselectedListener(UnityAction listener)
    {
        onDeselected.AddListener(listener);
    }

    public void RemoveDeselectedListener(UnityAction listener)
    {
        onDeselected.RemoveListener(listener);
    }

    public void AddHoveredListener(UnityAction listener)
    {
        onHovered.AddListener(listener);
    }

    public void RemoveHoveredListener(UnityAction listener)
    {
        onHovered.RemoveListener(listener);
    }

    public void AddUnhoveredListener(UnityAction listener)
    {
        onUnhovered.AddListener(listener);
    }

    public void RemoveUnoveredListener(UnityAction listener)
    {
        onUnhovered.RemoveListener(listener);
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
}
