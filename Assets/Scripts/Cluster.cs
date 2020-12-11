using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    public List<Resource> resources;
    private int size;
    public int Size { get { return size; } }
    public bool destroyed;

    public Cluster(int size)
    {
        resources = new List<Resource>(size);
        destroyed = false;
    }
}
