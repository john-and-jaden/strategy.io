using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private static HUD instance;
    public static HUD Instance { get { return instance; } }

    [SerializeField] private CanvasGroup buildingMenu;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SetBuildingMenuActive(bool active)
    {
        SetMenuActive(buildingMenu, active);
    }

    private void SetMenuActive(CanvasGroup menu, bool active)
    {
        menu.alpha = active ? 1 : 0;
        menu.blocksRaycasts = active;
        menu.interactable = active;
    }
}
