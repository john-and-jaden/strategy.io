using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Selectable
{
    private int clusterId;
    public int ClusterId
    {
        get { return clusterId; }
        set { clusterId = value; }
    }
    void Start()
    {
        SpawnIndicators();
    }

    void Update()
    {
        UpdateIndicators();
    }
}
