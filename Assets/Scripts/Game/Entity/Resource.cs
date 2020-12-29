using System.Collections.Generic;
using UnityEngine;

public class Resource : Selectable
{
    private Cluster cluster;
    public Cluster Cluster
    {
        get { return cluster; }
        set { cluster = value; }
    }

    [SerializeField] private ResourceDrop resourceDropPrefab;

    override protected void DestroySelf()
    {
        DestroyIndicators();
        cluster.Resources.Remove(this);
        onDestroyed.Invoke();
        Instantiate(resourceDropPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
