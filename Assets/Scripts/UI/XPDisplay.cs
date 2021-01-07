using UnityEngine;
using TMPro;

public class XPDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI XPText;

    void Start()
    {
        GameManager.ResourceSystem.AddWoodChangedListener(SetXPText);
    }

    private void SetXPText(int XP)
    {
        XPText.text = XP.ToString();
    }
}
