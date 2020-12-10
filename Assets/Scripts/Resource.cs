using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Selectable
{
    void Start()
    {
        SpawnIndicators();
    }

    void Update()
    {
        UpdateIndicators();
    }
}
