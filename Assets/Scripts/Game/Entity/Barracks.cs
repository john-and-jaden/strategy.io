using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    private UnitType[] unitTypes;

    public override void Select()
    {
        if (!interactive) return;

        HUD.ConstructionMenu.SetConstructionList<UnitType>(unitTypes);
        HUD.ConstructionMenu.Open();

        base.Select();
    }
}
