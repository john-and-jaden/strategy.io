using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Resource : Selectable
{
    public Cluster cluster;
    public float hardness;
    [System.Serializable] public class ResourceDiedEvent : UnityEvent { }
    private ResourceDiedEvent onResourceDied = new ResourceDiedEvent();
    void Start()
    {
        hardness = 60;
        SpawnIndicators();
    }

    void Update()
    {
        UpdateIndicators();
    }
    public void AddResourceDiedListened(UnityAction listener)
    {
        onResourceDied.AddListener(listener);
    }

    public void TakeDamage(float damageAmount)
    {
        hardness -= damageAmount;
        if (hardness <= 0)
        {
            SetHovered(false);
            UpdateIndicators();
            cluster.resources.Remove(this);
            Object.Destroy(this.gameObject);
            onResourceDied.Invoke();
        }
    }
}
