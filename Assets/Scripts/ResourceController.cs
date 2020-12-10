using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    // Public vars
    public Tree treePrefab;

    // Serialized fields
    [Tooltip("A multiplier for the maximum amount of resources per cluster from 1 to 10")]
    [Range(1, 10)] [SerializeField] private int clusterRichness = 5;
    public int ClusterRichness { get { return clusterRichness; } }
    [Tooltip("The frequency of resource clusters on the map from 1 to 10")]
    [Range(1, 10)] [SerializeField] private int clusterFrequency = 5;
    public int ClusterFrequency { get { return clusterFrequency; } }
    [Tooltip("The 'spread' of resources within a cluster from 1 to 10")]
    [Range(1, 10)] [SerializeField] private int clusterSparseness = 5;
    public int ClusterSparseness { get { return clusterSparseness; } }

    // Private vars
    public List<Cluster> clusters = new List<Cluster>();
    float halfWidth;
    float halfHeight;
    private Transform resourcesParent;

    void Start()
    {
        resourcesParent = new GameObject("Resource Clusters").transform;

        halfWidth = GetComponent<GridController>().GetDimensions().x / 2;
        halfHeight = GetComponent<GridController>().GetDimensions().y / 2;

        int clusterNum = 0;

        for (float i = -halfWidth + 0.5f; i < halfWidth + 0.5f; i++)
        {
            for (float j = -halfHeight + 0.5f; j < halfHeight + 0.5f; j++)
            {
                if (Random.Range(0f, 1f) > 1 - clusterFrequency / 1000f)
                {
                    GenerateCluster(i, j, treePrefab, clusterNum++);
                }
            }
        }
    }

    private void GenerateCluster(float clusterPosX, float clusterPosY, Resource resourcePrefab, int clusterNum)
    {
        int clusterSize = Random.Range(1, clusterRichness * 30);
        Cluster cluster = new Cluster(clusterSize, clusterNum);
        clusters.Add(cluster);

        string parentName = string.Format("Cluster [{0},{1}] ({2}) ", clusterPosX, clusterPosY, clusterSize);
        Transform clusterParent = new GameObject(parentName).transform;
        clusterParent.parent = resourcesParent;

        for (int resourceNum = 0; resourceNum < clusterSize; resourceNum++)
        {
            float distanceFromClusterCenterX = Random.Range(-resourceNum, resourceNum) * clusterSparseness / 150f;
            float distanceFromClusterCenterY = Random.Range(-resourceNum, resourceNum) * clusterSparseness / 150f;
            float resourcePosX = Mathf.Clamp(clusterPosX + distanceFromClusterCenterX, -halfWidth + 0.5f, halfWidth + 0.5f);
            float resourcePosY = Mathf.Clamp(clusterPosY + distanceFromClusterCenterY, -halfHeight + 0.5f, halfHeight + 0.5f);
            Resource resource = Instantiate(resourcePrefab, new Vector2(resourcePosX, resourcePosY), Quaternion.identity, clusterParent);
            resource.cluster = cluster;
            cluster.resources.Add(resource);
        }
    }
}
