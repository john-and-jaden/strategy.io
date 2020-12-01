using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable
{
    // Start is called before the first frame update
    void Start()
    {
        SpawnIndicators();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIndicators();
    }
}
