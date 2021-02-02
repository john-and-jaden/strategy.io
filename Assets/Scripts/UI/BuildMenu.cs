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

    public void SetBuildQueue<T>(T[] buildQueue) where T : InteractableType
    {
        if (buildQueue.Length > buildQueueIndicators.Length)
        {
            Debug.LogWarning($"Length of build queue exceeded menu capacity of {buildQueueIndicators.Length} indicators!");
        }

        // Hide all selectors
        for (int i = 0; i < buildQueueIndicators.Length; i++)
        {
            buildQueueIndicators[i].Close();
        }

        // Show and update however many selectors we need
        int numIndicators = Mathf.Min(buildQueue.Length, buildQueueIndicators.Length);
        for (int i = 0; i < numIndicators; i++)
        {
            buildQueueIndicators[i].SetInteractableType(buildQueue[i]);
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
        for (int i = 0; i < buildSelectors.Length; i++)
        {
            buildSelectors[i].Close();
        }

        // Show and update however many selectors we need
        numSelectorsActive = buildList.Length;
        for (int i = 0; i < numSelectorsActive; i++)
        {
            buildSelectors[i].SetInteractableType(buildList[i]);
            buildSelectors[i].Open();
        }
    }

    private void HandleResourceChanged(int resource)
    {
        for (int i = 0; i < numSelectorsActive; i++)
        {
            buildSelectors[i].UpdateAvailability();
        }
    }
}
