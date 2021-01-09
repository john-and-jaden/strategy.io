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

    public static T GetNearestIfAround<T>(Vector3 targetPos, float radius, Collider2D[] collidersFound, LayerMask layerMask) where T : Interactable
    {
        int collidersCount = Physics2D.OverlapCircleNonAlloc(targetPos, radius, collidersFound, layerMask);
        float shortestDistSqr = float.MaxValue;
        T nearest = null;
        for (int i = 0; i < collidersCount; i++)
        {
            if (collidersFound[i].TryGetComponent<T>(out T t))
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