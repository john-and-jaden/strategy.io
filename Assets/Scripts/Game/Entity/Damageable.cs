using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Damageable : Selectable
{
    [SerializeField] private SpriteRenderer healthBarPrefab;

    protected SpriteRenderer healthBar;

    [SerializeField] protected float initialDurability = 10;
    private float durability;
    public float Durability { get { return durability; } }
    [System.Serializable] public class DestroyedEvent : UnityEvent { }
    protected DestroyedEvent onDestroyed = new DestroyedEvent();
    protected bool damaged;

    new protected void Start()
    {
        durability = initialDurability;
        SpawnIndicators();
    }
    new protected void Update()
    {
        UpdateIndicators();
    }

    new protected void UpdateIndicators()
    {
        base.UpdateIndicators();
        healthBar.enabled = damaged;
    }

    new protected void SpawnIndicators()
    {
        base.SpawnIndicators();
        healthBar = Instantiate(healthBarPrefab, GameManager.SelectionSystem.IndicatorParent);
        healthBar.transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    }

    new protected void DestroyIndicators()
    {
        base.DestroyIndicators();
        Destroy(healthBar.gameObject);
    }
    public void TakeDamage(float damage)
    {
        damaged = true;
        durability -= damage;
        Vector3 healthBarScale = healthBar.transform.localScale;
        healthBar.size = new Vector2(durability / initialDurability, healthBar.size.y);
        if (durability <= 0)
        {
            DestroySelf();
        }
    }
    protected virtual void DestroySelf()
    {
        DestroyIndicators();
        onDestroyed.Invoke();
        Destroy(gameObject);
    }

    public void AddDestroyedListener(UnityAction listener)
    {
        onDestroyed.AddListener(listener);
    }

    public void RemoveDestroyedListener(UnityAction listener)
    {
        onDestroyed.RemoveListener(listener);
    }
}
