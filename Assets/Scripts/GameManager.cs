using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static SelectionSystem SelectionSystem { get { return selectionSystem; } }
    private static SelectionSystem selectionSystem;
    public static UnitSystem UnitSystem { get { return unitSystem; } }
    private static UnitSystem unitSystem;
    public static ResourceSystem ResourceSystem { get { return resourceSystem; } }
    private static ResourceSystem resourceSystem;

    private static GameManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        selectionSystem = GetComponent<SelectionSystem>();
        unitSystem = GetComponent<UnitSystem>();
        resourceSystem = GetComponent<ResourceSystem>();
    }
}
