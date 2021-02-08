using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : Building
{
    [System.Serializable] public class BuildQueueChangedEvent : UnityEvent<Queue<UnitType>> { }

    [SerializeField] private UnitType[] buildList;
    [SerializeField] private Transform buildSpawn;

    private Queue<UnitType> buildQueue = new Queue<UnitType>();
    private bool isBuildCycleActive;

    private BuildQueueChangedEvent onBuildQueueChanged = new BuildQueueChangedEvent();

    public override void Select()
    {
        if (!interactive) return;
        if (!completed) return;

        HUD.BuildMenu.SetBuildList<UnitType>(buildList);
        HUD.BuildMenu.SetBuildQueue<UnitType>(buildQueue);
        HUD.BuildMenu.Open();
        onBuildQueueChanged.AddListener(HUD.BuildMenu.SetBuildQueue);

        base.Select();
    }

    public override void Deselect()
    {
        if (!interactive) return;
        if (!completed) return;

        onBuildQueueChanged.RemoveListener(HUD.BuildMenu.SetBuildQueue);
        HUD.BuildMenu.Close();

        base.Deselect();
    }

    public void BuildUnit(UnitType unitType)
    {
        buildQueue.Enqueue(unitType);
        onBuildQueueChanged.Invoke(buildQueue);
        if (!isBuildCycleActive) StartCoroutine(BuildCycle());
    }

    private IEnumerator BuildCycle()
    {
        isBuildCycleActive = true;

        while (buildQueue.Count > 0)
        {
            UnitType current = buildQueue.Peek();
            yield return new WaitForSeconds(current.BuildTime);
            buildQueue.Dequeue();
            onBuildQueueChanged.Invoke(buildQueue);
            Instantiate(current.InteractablePrefab, buildSpawn.position, Quaternion.identity);
        }

        isBuildCycleActive = false;
    }
}
