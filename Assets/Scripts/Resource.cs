using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Resource : Selectable
{
    [System.Serializable] public class ResourceDestroyedEvent : UnityEvent { }
    [SerializeField] private Cluster cluster;
    public Cluster Cluster
    {
        get { return cluster; }
        set { cluster = value; }
    }

    [SerializeField] private float durability = 10;
    public float Durability { get { return durability; } }

    private ResourceDestroyedEvent onDestroyed = new ResourceDestroyedEvent();

    void Start()
    {
        SpawnIndicators();
    }

    void Update()
    {
        UpdateIndicators();
    }

    public void AddDestroyedListener(UnityAction listener)
    {
        onDestroyed.AddListener(listener);
    }

    public void RemoveDestroyedListener(UnityAction listener)
    {
        onDestroyed.RemoveListener(listener);
    }

    public void TakeDamage(float damage)
    {
        durability -= damage;
        if (durability <= 0)
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        DestroyIndicators();
        cluster.Resources.Remove(this);
        onDestroyed.Invoke();
        Destroy(gameObject);
    }
}
