using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    // Public vars and serialized fields
    public Tree treePrefab;
    public int ClusterRichness
    {
        get
        {
            return clusterRichness;
        }
    }
    [Range(1, 10)]
    [Tooltip("The maximum amount of resources per cluster from 0 to 10")]
    [SerializeField] private int clusterRichness = 5;
    public int ClusterFrequency
    {
        get
        {
            return clusterFrequency;
        }
    }
    [Range(1, 10)]
    [Tooltip("The frequency of resource clusters on the map from 1 to 10")]
    [SerializeField] private int clusterFrequency = 5;
    public int ClusterSparseness
    {
        get
        {
            return clusterSparseness;
        }
    }
    [Range(1, 10)]
    [Tooltip("The 'spread' of resources within a cluster from 1 to 10")]
    [SerializeField] private int clusterSparseness = 5;

    // Private vars
    private List<MonoBehaviour> resources = new List<MonoBehaviour>();

    int halfWidth;
    int halfHeight;
    void Start()
    {
        halfWidth = GetComponent<GridController>().GetDimensions().x / 2;
        halfHeight = GetComponent<GridController>().GetDimensions().y / 2;

        for (int i = -halfWidth; i < halfWidth; i++)
        {
            for (int j = -halfHeight; j < halfHeight; j++)
            {
                if (Random.Range(0f, 1f) > 1 - clusterFrequency / 1000f)
                {
                    GenerateCluster(i, j, treePrefab);
                }
            }
        }
    }

    private void GenerateCluster(float clusterPosX, float clusterPosY, MonoBehaviour resourcePrefab)
    {
        int clusterSize = Random.Range(1, clusterRichness * 30);
        for (int resourceNum = 0; resourceNum < clusterSize; resourceNum++)
        {
            float distanceFromClusterCenterX = Random.Range(-resourceNum, resourceNum) * clusterSparseness / 100f;
            float distanceFromClusterCenterY = Random.Range(-resourceNum, resourceNum) * clusterSparseness / 100f;
            float resourcePosX = Mathf.Clamp(clusterPosX + distanceFromClusterCenterX, -halfWidth + 0.5f, halfWidth + 0.5f);
            float resourcePosY = Mathf.Clamp(clusterPosY + distanceFromClusterCenterY, -halfHeight + 0.5f, halfHeight + 0.5f);
            resources.Add(Instantiate(resourcePrefab, new Vector3(resourcePosX, resourcePosY, 0), Quaternion.identity));
        }
    }
}
