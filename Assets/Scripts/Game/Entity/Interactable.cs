using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    private const float NON_INTERACTIVE_PERIOD = 0.1f;

    [System.Serializable] public class SelectedEvent : UnityEvent { }
    [System.Serializable] public class DeselectedEvent : UnityEvent { }
    [System.Serializable] public class HoveredEvent : UnityEvent { }
    [System.Serializable] public class UnhoveredEvent : UnityEvent { }
    [System.Serializable] public class DeathEvent : UnityEvent { }
    [System.Serializable] public class HealthChangedEvent : UnityEvent<float> { }
    [System.Serializable] public class MaxHealthChangedEvent : UnityEvent<float> { }

    [SerializeField] private SpriteRenderer hoverIndicator;
    [SerializeField] private SpriteRenderer selectIndicator;

    [SerializeField] protected float maxHealth = 10;
    public float MaxHealth { get { return maxHealth; } }

    protected float health;
    public float Health { get { return health; } }

    protected bool interactive = false;

    private SelectedEvent onSelected = new SelectedEvent();
    private DeselectedEvent onDeselected = new DeselectedEvent();
    private HoveredEvent onHovered = new HoveredEvent();
    private UnhoveredEvent onUnhovered = new UnhoveredEvent();
    private DeathEvent onDeath = new DeathEvent();
    private HealthChangedEvent onHealthChanged = new HealthChangedEvent();
    private MaxHealthChangedEvent onMaxHealthChanged = new MaxHealthChangedEvent();

    protected virtual void Start()
    {
        StartCoroutine(DelayInteractivity());
    }

    public virtual void Hover()
    {
        onHovered.Invoke();
        if (!interactive) return;
        if (hoverIndicator != null) hoverIndicator.enabled = true;
    }

    public virtual void Unhover()
    {
        onUnhovered.Invoke();
        if (!interactive) return;
        if (hoverIndicator != null) hoverIndicator.enabled = false;
    }

    public virtual void Select()
    {
        onSelected.Invoke();
        if (!interactive) return;
        if (selectIndicator != null) selectIndicator.enabled = true;
    }

    public virtual void Deselect()
    {
        onDeselected.Invoke();
        if (!interactive) return;
        if (selectIndicator != null) selectIndicator.enabled = false;
    }

    ///<summary>Returns true if the interactable died.</summary>
    public virtual bool TakeDamage(float damage)
    {
        UpdateHealth(-damage);
        bool dead = health <= 0;
        if (dead) Die();
        return dead;
    }

    ///<summary>Returns true if the interactable reached max health.</summary>
    public virtual bool GainHealth(float gain)
    {
        UpdateHealth(gain);
        return health >= maxHealth;
    }

    private void UpdateHealth(float deltaHealth)
    {
        float clampedDeltaHealth = Mathf.Clamp(deltaHealth, -health, maxHealth - health);
        if (clampedDeltaHealth != 0) onHealthChanged.Invoke(health);
        health += clampedDeltaHealth;
    }

    protected virtual void Die()
    {
        onDeath.Invoke();
        Destroy(gameObject);
    }

    private IEnumerator DelayInteractivity()
    {
        interactive = false;
        yield return new WaitForSeconds(NON_INTERACTIVE_PERIOD);
        interactive = true;
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

    public void RemoveUnhoveredListener(UnityAction listener)
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
