using UnityEngine;
using Mirror;

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

    [SyncVar(hook = nameof(HandleZIndexChanged))]
    private int zIndex;

    protected override void Awake()
    {
        playerId = -1;
        health = maxHealth;
        base.Awake();
    }

    override protected void Die()
    {
        cluster.Resources.Remove(this);
        SpawnResourceDrops();
        base.Die();
    }

    [Server]
    public void Initialize(int zIndex, Cluster cluster)
    {
        this.zIndex = zIndex;
        this.cluster = cluster;
    }

    private void SpawnResourceDrops()
    {
        for (int i = 0; i < resourceDropCount; i++)
        {
            ResourceDrop drop = Instantiate(resourceDropPrefab, transform.position, Quaternion.identity);
            Vector2 popForce = Random.insideUnitCircle * resourceDropMaxPopForce;
            drop.GetComponent<Rigidbody2D>().AddForce(popForce, ForceMode2D.Impulse);
        }
    }

    private void HandleZIndexChanged(int oldZIndex, int newZIndex)
    {
        GetComponent<SpriteRenderer>().sortingOrder = newZIndex;
    }
}
