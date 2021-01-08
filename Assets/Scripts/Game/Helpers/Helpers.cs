using UnityEngine;
using System.Collections.Generic;

public class Helpers
{
    public static T GetNearestInteractable<T>(List<T> interactables, Vector2 targetPos) where T : Interactable
    {
        float minDistance = float.MaxValue;
        T closest = null;
        foreach (T interactable in interactables)
        {
            float distance = Vector3.Distance(targetPos, interactable.transform.position);
            if (minDistance > distance)
            {
                minDistance = distance;
                closest = interactable;
            }
        }
        return closest;
    }
}