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
    [SerializeField] private BuildQueueChangedEvent onBuildQueueChanged = new BuildQueueChangedEvent();
    [SerializeField] private BuildProgressChangedEvent onBuildProgressChanged = new BuildProgressChangedEvent();

    private Queue<UnitType> buildQueue = new Queue<UnitType>();
    private bool isBuildCycleActive;

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
        // Update the build queue
        buildQueue.Enqueue(unitType);
        onBuildQueueChanged.Invoke(buildQueue);

        // Expend resources
        GameManager.ResourceSystem.SpendWood(unitType.WoodCost);
        GameManager.ResourceSystem.SpendStone(unitType.StoneCost);

        // Start build cycle if not already active
        if (!isBuildCycleActive) StartCoroutine(BuildCycle());
    }

    private IEnumerator BuildCycle()
    {
        isBuildCycleActive = true;

        while (buildQueue.Count > 0)
        {
            // Wait for the build time
            UnitType current = buildQueue.Peek();
            yield return StartCoroutine(ProcessBuild(current));

            // Create the unit gameobject
            Instantiate(current.InteractablePrefab, buildSpawn.position, Quaternion.identity);

            // Update the build queue
            buildQueue.Dequeue();
            onBuildQueueChanged.Invoke(buildQueue);
        }

        isBuildCycleActive = false;
    }

    private IEnumerator ProcessBuild(UnitType unitType)
    {
        float elapsed = 0f;
        while (elapsed < unitType.BuildTime)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp(elapsed / unitType.BuildTime, 0f, 1f);
            onBuildProgressChanged.Invoke(progress);

            yield return null;
        }
    }
}
