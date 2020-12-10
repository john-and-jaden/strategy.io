using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    public List<Resource> resources;
    public int Size { get { return size; } }
    private int size;

    public Cluster(int size)
    {
        this.size = size;
        resources = new List<Resource>();
    }

    void Start()
    {

    }
}
