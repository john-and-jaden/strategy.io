using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Building
{
    [SerializeField] private UnitType[] buildList;
    [SerializeField] private Transform buildSpawn;

    private Queue<UnitType> buildQueue = new Queue<UnitType>();
    private bool isBuildCycleActive;

    public override void Select()
    {
        if (!interactive) return;

        HUD.BuildMenu.SetBuildList<UnitType>(buildList);
        HUD.BuildMenu.Open();

        base.Select();
    }

    public override void Deselect()
    {
        if (!interactive) return;

        HUD.BuildMenu.Close();

        base.Deselect();
    }

    public void BuildUnit(UnitType unitType)
    {
        buildQueue.Enqueue(unitType);
        HUD.BuildMenu.SetBuildQueue<UnitType>(buildQueue.ToArray());
        if (!isBuildCycleActive) StartCoroutine(BuildCycle());
    }

    private IEnumerator BuildCycle()
    {
        isBuildCycleActive = true;

        while (buildQueue.Count > 0)
        {
            UnitType current = buildQueue.Dequeue();
            HUD.BuildMenu.SetBuildQueue<UnitType>(buildQueue.ToArray());
            yield return new WaitForSeconds(current.BuildTime);
            Instantiate(current.InteractablePrefab, buildSpawn.position, Quaternion.identity);
        }

        isBuildCycleActive = false;
    }
}
