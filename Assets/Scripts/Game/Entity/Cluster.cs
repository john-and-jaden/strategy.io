using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    private List<Resource> resources;
    public List<Resource> Resources { get { return resources; } }

    public Cluster(int size)
    {
        resources = new List<Resource>(size);
    }
}
