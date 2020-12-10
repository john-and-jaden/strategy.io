using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Selectable
{
    public Cluster cluster;
    public float hardness;
    void Start()
    {
        SpawnIndicators();
        hardness = 60;
    }

    void Update()
    {
        if (hardness <= 0)
        {
            SetHovered(false);
            cluster.resources.Remove(this);
            if (cluster.resources.Count == 0)
            {
                cluster.destroyed = true;
                cluster = null;
            }
            Object.Destroy(this.gameObject);
        }
        UpdateIndicators();
    }
}
