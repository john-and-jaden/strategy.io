using System.Collections.Generic;

public class Cluster
{
    private List<Resource> resources;
    public List<Resource> Resources { get { return resources; } }

    public Cluster(int size)
    {
        resources = new List<Resource>(size);
    }
}
