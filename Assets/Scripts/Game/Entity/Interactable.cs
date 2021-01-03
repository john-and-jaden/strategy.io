using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [System.Serializable] public class DestroyedEvent : UnityEvent { }

    [SerializeField] private SpriteRenderer hoverIndicatorPrefab;
    [SerializeField] private SpriteRenderer selectIndicatorPrefab;
    [SerializeField] private SpriteRenderer healthBarPrefab;
    [SerializeField] protected float healthBarOffset = 0.7f;
    [SerializeField] private float healthBarFadeDelay = 2f;
    [SerializeField] private float maxHealth = 10;

    private float health;
    public float Health { get { return health; } }
    // The below public field should be deleted and replaced with the two lines below once we build multiplayer
    public int playerId = 1;
    // private int playerId;
    // public int PlayerId { get { return playerId; } set { playerId = value; } }

    protected bool hovered;
    protected bool selected;
    protected SpriteRenderer hoverIndicator;
    protected SpriteRenderer selectIndicator;
    protected SpriteRenderer healthBar;

    private float healthBarFadeTimer = 0f;

    protected DestroyedEvent onDestroyed = new DestroyedEvent();

    protected void SpawnIndicators()
    {
        hoverIndicator = Instantiate(hoverIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        selectIndicator = Instantiate(selectIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        hoverIndicator.transform.position = transform.position;
        selectIndicator.transform.position = transform.position;

        Vector2 healthBarPos = transform.position + Vector3.up * healthBarOffset;
        healthBar = Instantiate(healthBarPrefab, healthBarPos, Quaternion.identity, GameManager.SelectionSystem.IndicatorParent);
        healthBar.enabled = false;
    }

    protected void UpdateIndicators()
    {
        hoverIndicator.enabled = hovered;
        selectIndicator.enabled = selected;
    }

    protected void DestroyIndicators()
    {
        Destroy(hoverIndicator.gameObject);
        Destroy(selectIndicator.gameObject);
        Destroy(healthBar.gameObject);
    }

    protected void Start()
    {
        health = maxHealth;
        SpawnIndicators();
    }

    protected void Update()
    {
        UpdateIndicators();

        healthBarFadeTimer += Time.deltaTime;
        if (healthBarFadeTimer >= healthBarFadeDelay)
        {
            healthBar.enabled = false;
        }
    }

    public void SetHovered(bool hovered)
    {
        this.hovered = hovered;
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) DestroySelf();

        healthBarFadeTimer = 0f;
        healthBar.size = new Vector2(health / maxHealth, healthBar.size.y);
        healthBar.enabled = true;
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
