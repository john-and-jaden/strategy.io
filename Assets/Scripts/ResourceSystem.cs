using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    public int Wood { get { return wood; } }
    private int wood;
    public int Stone { get { return stone; } }
    private int stone;

    public void AddWood(int wood)
    {
        this.wood += wood;
        Debug.Log(this.wood);
    }

    public void AddStone(int stone)
    {
        this.stone += stone;
        Debug.Log(this.stone);
    }
}
