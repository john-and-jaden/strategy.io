using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private static BuildingMenu buildingMenu;
    public static BuildingMenu BuildingMenu { get { return buildingMenu; } }

    private static HUD instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        buildingMenu = GetComponentInChildren<BuildingMenu>();
    }
}
