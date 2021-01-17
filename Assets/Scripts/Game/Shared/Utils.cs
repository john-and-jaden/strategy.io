using UnityEngine;

public static class Utils
{
    public delegate bool MatchCondition(Interactable t);

    private static Collider2D[] pool = new Collider2D[10];

    public static T GetNearest<T>(Vector3 pos, float radius, LayerMask mask) where T : Interactable
    {
        return GetNearest<T>(pos, radius, mask, (t) => true);
    }
    public static T GetNearest<T>(Vector3 pos, float radius, LayerMask mask, MatchCondition matchCondition) where T : Interactable
    {
        int numTargets = Physics2D.OverlapCircleNonAlloc(pos, radius, pool, mask);
        float shortestDistSqr = float.MaxValue;
        T nearest = null;
        for (int i = 0; i < numTargets; i++)
        {
            if (pool[i].TryGetComponent<T>(out T t) && matchCondition(t))
            {
                float tDistSqr = (t.transform.position - pos).sqrMagnitude;
                if (tDistSqr < shortestDistSqr)
                {
                    shortestDistSqr = tDistSqr;
                    nearest = t;
                }
            }
        }
        return nearest;
    }
}