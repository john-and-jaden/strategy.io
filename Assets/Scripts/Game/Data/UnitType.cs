using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UnitType", menuName = "UnitType", order = 52)]
public class UnitType : InteractableType
{
    [SerializeField] private float buildTime;
    public float BuildTime { get { return buildTime; } }

    public override void HandleSelect()
    {
        //unit stuff
    }
}
