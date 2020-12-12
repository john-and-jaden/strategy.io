using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Resource : Selectable
{
    [System.Serializable] public class ResourceDiedEvent : UnityEvent { }
    [SerializeField] private Cluster cluster;
    public Cluster Cluster
    {
        get { return cluster; }
        set { cluster = value; }
    }

    private float health;
    private ResourceDiedEvent onResourceDied = new ResourceDiedEvent();

    void Start()
    {
        health = 60;
        SpawnIndicators();
    }

    void Update()
    {
        UpdateIndicators();
    }

    public void AddResourceDiedListener(UnityAction listener)
    {
        onResourceDied.AddListener(listener);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        SetHovered(false);
        UpdateIndicators();
        cluster.Resources.Remove(this);
        Object.Destroy(this.gameObject);
        onResourceDied.Invoke();
    }
}
