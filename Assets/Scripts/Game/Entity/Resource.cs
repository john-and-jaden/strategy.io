using System.Collections.Generic;
using UnityEngine;

public class Resource : Interactable
{
    [SerializeField] private ResourceDrop resourceDropPrefab;
    [SerializeField] private int resourceDropCount = 2;
    [SerializeField] private float resourceDropMaxPopForce = 5f;

    private Cluster cluster;
    public Cluster Cluster
    {
        get { return cluster; }
        set { cluster = value; }
    }

    override protected void DestroySelf()
    {
        DestroyIndicators();
        cluster.Resources.Remove(this);
        onDestroyed.Invoke();
        SpawnResourceDrops();
        Destroy(gameObject);
    }

    private void SpawnResourceDrops()
    {
        for (int i = 0; i < resourceDropCount; i++)
        {
            ResourceDrop drop = Instantiate(resourceDropPrefab, transform.position, Quaternion.identity);
            Vector2 forceDir = Random.insideUnitCircle.normalized;
            float forceScale = Random.Range(0, 1f);
            Vector2 popForce = forceDir * forceScale * resourceDropMaxPopForce;
            drop.GetComponent<Rigidbody2D>().AddForce(popForce, ForceMode2D.Impulse);
        }
    }
}
