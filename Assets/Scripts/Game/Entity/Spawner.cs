using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : Building
{
    [System.Serializable] public class BuildQueueChangedEvent : UnityEvent<Queue<UnitType>> { }
    [System.Serializable] public class BuildProgressChangedEvent : UnityEvent<float> { }

    [SerializeField] private UnitType[] buildList;
    [SerializeField] private Transform buildSpawn;

    private Queue<UnitType> buildQueue = new Queue<UnitType>();
    private bool isBuildCycleActive;

    private BuildQueueChangedEvent onBuildQueueChanged = new BuildQueueChangedEvent();
    private BuildProgressChangedEvent onBuildProgressChanged = new BuildProgressChangedEvent();

    public override void Select()
    {
        if (!interactive) return;
        if (!completed) return;

        HUD.BuildMenu.SetBuildList<UnitType>(buildList);
        HUD.BuildMenu.SetBuildQueue<UnitType>(buildQueue);
        HUD.BuildMenu.Open();
        onBuildQueueChanged.AddListener(HUD.BuildMenu.SetBuildQueue);
        onBuildProgressChanged.AddListener(HUD.BuildMenu.SetBuildProgress);

        base.Select();
    }

    public override void Deselect()
    {
        if (!interactive) return;
        if (!completed) return;

        onBuildProgressChanged.RemoveListener(HUD.BuildMenu.SetBuildProgress);
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
            yield return StartCoroutine(ProcessBuild(current));

            buildQueue.Dequeue();
            onBuildQueueChanged.Invoke(buildQueue);
            Instantiate(current.InteractablePrefab, buildSpawn.position, Quaternion.identity);
        }

        isBuildCycleActive = false;
    }

    private IEnumerator ProcessBuild(UnitType unitType)
    {
        float elapsed = 0f;
        while (elapsed < unitType.BuildTime)
        {
            yield return null;
            elapsed += Time.deltaTime;
            onBuildProgressChanged.Invoke(elapsed / unitType.BuildTime);
        }
    }
}
