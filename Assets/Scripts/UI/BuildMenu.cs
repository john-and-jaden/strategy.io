using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : HUDMenu
{
    [SerializeField] private BuildSelector[] buildSelectors;
    [SerializeField] private BuildQueueIndicator[] buildQueueIndicators;

    private int numSelectorsActive;
    private int numQueueIndicatorsActive;

    protected void Start()
    {
        GameManager.ResourceSystem.AddWoodChangedListener(HandleResourceChanged);
        GameManager.ResourceSystem.AddStoneChangedListener(HandleResourceChanged);
    }

    public void SetBuildQueue<T>(Queue<T> buildQueue) where T : InteractableType
    {
        if (buildQueue.Count > buildQueueIndicators.Length)
        {
            Debug.LogWarning($"Length of build queue exceeded menu capacity of {buildQueueIndicators.Length} indicators!");
        }

        // Hide all indicators
        HideIndicators();

        // Show and update however many indicators we need
        int numIndicators = Mathf.Min(buildQueue.Count, buildQueueIndicators.Length);
        T[] buildQueueArray = buildQueue.ToArray();
        for (int i = 0; i < numIndicators; i++)
        {
            buildQueueIndicators[i].SetInteractableType(buildQueueArray[i]);
            buildQueueIndicators[i].Open();
        }
    }

    public void SetBuildList<T>(T[] buildList) where T : InteractableType
    {
        if (buildList.Length > buildSelectors.Length)
        {
            Debug.LogError($"Length of build list exceeded menu capacity of {buildSelectors.Length} selectors!");
            return;
        }

        // Hide all selectors
        HideSelectors();

        // Show and update however many selectors we need
        numSelectorsActive = buildList.Length;
        for (int i = 0; i < numSelectorsActive; i++)
        {
            buildSelectors[i].SetInteractableType(buildList[i]);
            buildSelectors[i].Open();
        }
    }

    public override void Close()
    {
        base.Close();
        HideIndicators();
        HideSelectors();
    }

    private void HandleResourceChanged(int resource)
    {
        for (int i = 0; i < numSelectorsActive; i++)
        {
            buildSelectors[i].UpdateAvailability();
        }
    }

    private void HideIndicators()
    {
        for (int i = 0; i < buildQueueIndicators.Length; i++)
        {
            buildQueueIndicators[i].Close();
        }
    }

    private void HideSelectors()
    {
        for (int i = 0; i < buildSelectors.Length; i++)
        {
            buildSelectors[i].Close();
        }
    }
}
