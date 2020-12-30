using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDrop : ResourceDrop
{
    protected override void Collect()
    {
        GameManager.ResourceSystem.AddWood(resourceAmount);
    }
}
