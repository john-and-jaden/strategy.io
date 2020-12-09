using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceSystem : MonoBehaviour
{
    [System.Serializable] public class WoodChangedEvent : UnityEvent<int> { }
    [System.Serializable] public class StoneChangedEvent : UnityEvent<int> { }

    public int Wood { get { return wood; } }
    private int wood;
    public int Stone { get { return stone; } }
    private int stone;

    private WoodChangedEvent onWoodChanged = new WoodChangedEvent();
    private StoneChangedEvent onStoneChanged = new StoneChangedEvent();

    void Update() 
    {
        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddWood(1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddStone(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpendWood(1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpendStone(1);
        }
    }

    public void AddWood(int amount)
    {
        wood += amount;
        onWoodChanged.Invoke(wood);
    }

    public void AddStone(int amount)
    {
        stone += amount;
        onStoneChanged.Invoke(stone);
    }

    public void SpendWood(int amount)
    {
        if (wood - amount >= 0)
        {
            wood -= amount;
            onWoodChanged.Invoke(wood);
        }
    }

    public void SpendStone(int amount)
    {
        if (stone - amount >= 0)
        {
            stone -= amount;
            onStoneChanged.Invoke(stone);
        }
    }

    public void AddWoodChangedListener(UnityAction<int> listener)
    {
        onWoodChanged.AddListener(listener);
    }

    public void RemoveWoodChangedListener(UnityAction<int> listener)
    {
        onWoodChanged.RemoveListener(listener);
    }

    public void AddStoneChangedListener(UnityAction<int> listener)
    {
        onStoneChanged.AddListener(listener);
    }

    public void RemoveStoneChangedListener(UnityAction<int> listener)
    {
        onStoneChanged.RemoveListener(listener);
    }
}
