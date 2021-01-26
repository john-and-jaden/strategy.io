using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private static ConstructionMenu constructionMenu;
    public static ConstructionMenu ConstructionMenu { get { return constructionMenu; } }

    private static HUD instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        constructionMenu = GetComponentInChildren<ConstructionMenu>();
    }
}
