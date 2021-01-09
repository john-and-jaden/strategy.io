using UnityEngine;
using System.Collections.Generic;

public static class Helper
{
    public static T GetNearestInList<T>(List<T> interactables, Vector2 targetPos) where T : Interactable
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

    public static T GetNearestIfAround<T>(Vector3 targetPos, float radius, int resultsSize, LayerMask layerMask, int playerId = -2) where T : Interactable
    {
        Collider2D[] results = new Collider2D[resultsSize];
        Physics2D.OverlapCircleNonAlloc(targetPos, radius, results, layerMask);
        float shortestDistSqr = float.MaxValue;
        T nearest = null;
        foreach (Collider2D result in results)
        {
            if (result.TryGetComponent<T>(out T t))
            {
                float tDistSqr = (t.transform.position - targetPos).sqrMagnitude;
                if (tDistSqr < shortestDistSqr)
                {
                    nearest = t;
                    shortestDistSqr = tDistSqr;
                }
            }
        }
        return nearest;
    }
}