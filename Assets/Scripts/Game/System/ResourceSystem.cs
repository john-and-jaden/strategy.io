using UnityEngine;
using UnityEngine.Events;

public class ResourceSystem : MonoBehaviour
{
    [System.Serializable] public class WoodChangedEvent : UnityEvent<int> { }
    [System.Serializable] public class StoneChangedEvent : UnityEvent<int> { }

    [SerializeField] private Tree treePrefab;
    [SerializeField] private Stone stonePrefab;

    [Tooltip("A multiplier for the maximum amount of resources per cluster from 1 to 10")]
    [SerializeField] [Range(1, 10)] private int clusterRichness = 5;
    public int ClusterRichness { get { return clusterRichness; } }

    [Tooltip("The frequency of resource clusters on the map from 1 to 10")]
    [SerializeField] [Range(1, 10)] private int clusterFrequency = 5;
    public int ClusterFrequency { get { return clusterFrequency; } }

    [Tooltip("The 'spread' of resources within a cluster from 1 to 10")]
    [SerializeField] [Range(1, 10)] private int clusterSparseness = 5;
    public int ClusterSparseness { get { return clusterSparseness; } }

    private int wood;
    public int Wood { get { return wood; } }
    private int stone;
    public int Stone { get { return stone; } }

    private float halfWidth, halfHeight;
    private Transform resourcesParent;

    private WoodChangedEvent onWoodChanged = new WoodChangedEvent();
    private StoneChangedEvent onStoneChanged = new StoneChangedEvent();

    void Start()
    {
        resourcesParent = new GameObject("Resource Clusters").transform;

        halfWidth = GameManager.GridSystem.GetDimensions().x / 2;
        halfHeight = GameManager.GridSystem.GetDimensions().y / 2;

        for (float i = -halfWidth + 0.5f; i < halfWidth + 0.5f; i++)
        {
            for (float j = -halfHeight + 0.5f; j < halfHeight + 0.5f; j++)
            {
                // Determine whether to spawn a cluster at the current location
                if (Random.Range(0f, 1f) > 1 - clusterFrequency / 1000f)
                {
                    // Determine which type of resource to spawn
                    Resource r;
                    if (Random.value < 0.5f) r = treePrefab;
                    else r = stonePrefab;
                    GenerateCluster(i, j, r);
                }
            }
        }
    }

    void Update()
    {
        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddWood(1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddStone(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpendWood(1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpendStone(1);
        }
    }

    public void AddWood(int amount)
    {
        wood += amount;
        onWoodChanged.Invoke(wood);
    }

    public void AddStone(int amount)
    {
        stone += amount;
        onStoneChanged.Invoke(stone);
    }

    public void SpendWood(int amount)
    {
        if (wood - amount >= 0)
        {
            wood -= amount;
            onWoodChanged.Invoke(wood);
        }
    }

    public void SpendStone(int amount)
    {
        if (stone - amount >= 0)
        {
            stone -= amount;
            onStoneChanged.Invoke(stone);
        }
    }

    public void AddWoodChangedListener(UnityAction<int> listener)
    {
        onWoodChanged.AddListener(listener);
    }

    public void RemoveWoodChangedListener(UnityAction<int> listener)
    {
        onWoodChanged.RemoveListener(listener);
    }

    public void AddStoneChangedListener(UnityAction<int> listener)
    {
        onStoneChanged.AddListener(listener);
    }

    public void RemoveStoneChangedListener(UnityAction<int> listener)
    {
        onStoneChanged.RemoveListener(listener);
    }

    private void GenerateCluster(float clusterPosX, float clusterPosY, Resource resourcePrefab)
    {
        // Initialize cluster object
        int clusterSize = Random.Range(1, clusterRichness * 30);
        Cluster cluster = new Cluster(clusterSize);

        // Add cluster parent transforms for better scene organization
        string parentName = string.Format("Cluster [{0},{1}] ({2}) ", clusterPosX, clusterPosY, clusterSize);
        Transform clusterParent = new GameObject(parentName).transform;
        clusterParent.parent = resourcesParent;

        // Spawn resources and fill cluster
        for (int resourceNum = 0; resourceNum < clusterSize; resourceNum++)
        {
            float distanceFromClusterCenterX = Random.Range(-resourceNum, resourceNum) * clusterSparseness / 150f;
            float distanceFromClusterCenterY = Random.Range(-resourceNum, resourceNum) * clusterSparseness / 150f;
            float resourcePosX = Mathf.Clamp(clusterPosX + distanceFromClusterCenterX, -halfWidth + 0.5f, halfWidth + 0.5f);
            float resourcePosY = Mathf.Clamp(clusterPosY + distanceFromClusterCenterY, -halfHeight + 0.5f, halfHeight + 0.5f);
            Resource resource = Instantiate(resourcePrefab, new Vector2(resourcePosX, resourcePosY), Quaternion.identity, clusterParent);
            resource.GetComponent<SpriteRenderer>().sortingOrder = resourceNum;
            resource.Cluster = cluster;
            cluster.Resources.Add(resource);
        }
    }
}
