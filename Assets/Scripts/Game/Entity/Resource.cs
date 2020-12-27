using System.Collections;
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

    // new public void TakeDamage(float damage)
    // {
    //     damaged = true;
    //     durability -= damage;
    //     if (durability <= 0)
    //     {
    //         DestroySelf();
    //     }
    // }

    override protected void DestroySelf()
    {
        DestroyIndicators();
        cluster.Resources.Remove(this);
        onDestroyed.Invoke();
        Destroy(gameObject);
    }
}
