using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WoodDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI woodText;

    void Start()
    {
        GameManager.ResourceSystem.AddWoodChangedListener(SetWoodText);
    }

    private void SetWoodText(int wood)
    {
        woodText.text = wood.ToString();
    }
}
