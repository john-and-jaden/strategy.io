using System.Collections.Generic;
using UnityEngine;

public class Resource : Damageable
{
    [SerializeField] private Cluster cluster;
    public Cluster Cluster
    {
        get { return cluster; }
        set { cluster = value; }
    }

    [SerializeField] private PickupableResource piackableResourcePrefab;

    override protected void DestroySelf()
    {
        DestroyIndicators();
        cluster.Resources.Remove(this);
        onDestroyed.Invoke();
        Instantiate(piackableResourcePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
