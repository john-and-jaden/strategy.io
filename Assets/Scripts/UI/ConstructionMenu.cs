using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionMenu : HUDMenu
{
    [SerializeField] private ConstructionSelector[] selectors;

    private int numActive;

    protected void Start()
    {
        GameManager.ResourceSystem.AddWoodChangedListener(HandleResourceChanged);
        GameManager.ResourceSystem.AddStoneChangedListener(HandleResourceChanged);
    }

    public void SetConstructionList<T>(T[] interactableTypes) where T : InteractableType
    {
        if (interactableTypes.Length > selectors.Length)
        {
            Debug.LogError($"Number of building types exceeded menu capacity of {selectors.Length} building selectors!");
            return;
        }

        // Hide all selectors
        for (int i = 0; i < selectors.Length; i++)
        {
            selectors[i].Close();
        }

        // Show and update however many selectors we need
        numActive = interactableTypes.Length;
        for (int i = 0; i < numActive; i++)
        {
            selectors[i].SetInteractableType(interactableTypes[i]);
            selectors[i].Open();
        }
    }

    private void HandleResourceChanged(int resource)
    {
        for (int i = 0; i < numActive; i++)
        {
            selectors[i].UpdateAvailability();
        }
    }
}
