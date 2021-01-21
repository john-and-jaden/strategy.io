using UnityEngine;
using TMPro;

public class XpDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI xpText;

    void Start()
    {
        GameManager.XpSystem.AddXpChangedListener(SetXpText);
    }

    private void SetXpText(int xp)
    {
        xpText.text = xp.ToString();
    }
}
