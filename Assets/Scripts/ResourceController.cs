using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public Tree treePrefab;
    public int ClusterRichness
    {
        get
        {
            return clusterRichness;
        }
    }
    [Tooltip("The maximum amount of resources per cluster from 0 to 10")]
    [SerializeField] private int clusterRichness;
    public int ClusterFrequency
    {
        get
        {
            return clusterFrequency;
        }
    }
    [Tooltip("The frequency of resource clusters on the map from 1 to 10")]
    [SerializeField] private int clusterFrequency;
    public int ClusterSparseness
    {
        get
        {
            return clusterSparseness;
        }
    }
    [Tooltip("The 'spread' of resources within a cluster from 1 to 10")]
    [SerializeField] private int clusterSparseness;

    private List<MonoBehaviour> resources = new List<MonoBehaviour>();

    void Start()
    {
        int halfWidth = (int)GetComponent<GridController>().GetDimensions().x / 2;
        int halfHeight = (int)GetComponent<GridController>().GetDimensions().y / 2;
        for (int i = -halfWidth + 1; i < halfWidth + 1; i++)
        {
            for (int j = -halfHeight + 1; j < halfHeight + 1; j++)
            {
                if (Random.Range(0f, 1f) > 1 - clusterFrequency / 1000f)
                {
                    GenerateCluster(i, j, treePrefab);
                }
            }
        }
    }

    private void GenerateCluster(float xPos, float yPos, MonoBehaviour resourcePrefab)
    {
        int maxNumResources = Random.Range(1, clusterRichness * 30);
        for (int treeNum = 0; treeNum < maxNumResources; treeNum++)
        {
            float distanceFromClusterCenterX = Random.Range(-treeNum, treeNum);
            float distanceFromClusterCenterY = Random.Range(-treeNum, treeNum);
            distanceFromClusterCenterX /= 5 * clusterSparseness;
            distanceFromClusterCenterY /= 5 * clusterSparseness;
            resources.Add(Instantiate(resourcePrefab, new Vector3(xPos + distanceFromClusterCenterX, yPos + distanceFromClusterCenterY, 0), Quaternion.identity));
        }
    }
}
