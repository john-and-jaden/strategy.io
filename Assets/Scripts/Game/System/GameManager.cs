using UnityEngine;
using Mirror;

public class GameManager : NetworkManager
{
    private static SelectionSystem selectionSystem;
    public static SelectionSystem SelectionSystem { get { return selectionSystem; } }
    private static UnitSystem unitSystem;
    public static UnitSystem UnitSystem { get { return unitSystem; } }
    private static GridSystem gridSystem;
    public static GridSystem GridSystem { get { return gridSystem; } }
    private static ResourceSystem resourceSystem;
    public static ResourceSystem ResourceSystem { get { return resourceSystem; } }
    private static XpSystem xpSystem;
    public static XpSystem XpSystem { get { return xpSystem; } }
    private static BuildingSystem buildingSystem;
    public static BuildingSystem BuildingSystem { get { return buildingSystem; } }

    private static GameManager instance;

    public override void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        selectionSystem = GetComponent<SelectionSystem>();
        unitSystem = GetComponent<UnitSystem>();
        gridSystem = GetComponent<GridSystem>();
        resourceSystem = GetComponent<ResourceSystem>();
        xpSystem = GetComponent<XpSystem>();
        buildingSystem = GetComponent<BuildingSystem>();
    }

    public override void OnStartServer()
    {
        Debug.Log("Server started");
        resourceSystem.SpawnResources();
    }

    public override void OnStartClient()
    {
        Debug.Log("Client started");
    }
}
