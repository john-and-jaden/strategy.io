using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    public List<Resource> resources;
    private int size;
    public int Size { get { return size; } }
    private int id;
    public int Id { get { return id; } }

    public Cluster(int size, int id)
    {
        this.size = size;
        this.id = id;
        resources = new List<Resource>();
    }

    void Start()
    {
        
    }
}
