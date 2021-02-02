using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private static BuildMenu buildMenu;
    public static BuildMenu BuildMenu { get { return buildMenu; } }

    private static HUD instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        buildMenu = GetComponentInChildren<BuildMenu>();
    }
}
