using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public Tree treePrefab;

    private List<Tree> trees;
    void Start()
    {
        trees = new List<Tree>();
        for (int i = 0; i < 10; i++)
        {
            trees.Add(Instantiate(treePrefab, new Vector3(10, 10, 0), Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
