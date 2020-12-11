using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    public List<Resource> resources;
    public bool destroyed;

    public Cluster(int size)
    {
        resources = new List<Resource>(size);
        destroyed = false;
    }
}
