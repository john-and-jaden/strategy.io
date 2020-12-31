using UnityEngine;
using TMPro;

public class StoneDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stoneText;

    void Start()
    {
        GameManager.ResourceSystem.AddStoneChangedListener(SetStoneText);
    }

    private void SetStoneText(int stone)
    {
        stoneText.text = stone.ToString();
    }
}
