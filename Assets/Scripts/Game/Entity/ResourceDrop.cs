using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceDrop : MonoBehaviour
{
    private const int MAX_ATTRACTION_TARGETS = 10;
    private const float DESTROY_DIST_SQR = 0.1f;

    [SerializeField] protected int resourceAmount = 1;
    [SerializeField] private float attractionRadius = 1f;
    [SerializeField] private float attractionForce = 1f;
    [SerializeField] private LayerMask attractionMask;

    private new Rigidbody2D rigidbody2D;

    protected void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected void FixedUpdate()
    {
        Worker nearestWorker = Helper.GetNearestIfAround<Worker>(transform.position, attractionRadius, MAX_ATTRACTION_TARGETS, attractionMask);
        if (nearestWorker != null)
        {
            // Get vector towards the nearest worker
            Vector2 towardsWorker = nearestWorker.transform.position - transform.position;

            // If we are close enough, collect this drop
            float workerDistSqr = towardsWorker.sqrMagnitude;
            if (workerDistSqr < DESTROY_DIST_SQR)
            {
                Collect();
                Destroy(gameObject);
                return;
            }

            // Magnetize towards nearest worker
            Vector2 forceDir = towardsWorker.normalized;
            rigidbody2D.AddForce(forceDir * attractionForce, ForceMode2D.Impulse);
        }
    }

    protected abstract void Collect();
}