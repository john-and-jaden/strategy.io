using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSystem : MonoBehaviour
{
    // Approximation of the area of a Unit with radius 0.5
    private const float UNIT_AREA = 0.8f;

    [SerializeField] private float minGroupRadiusSqr = 0.25f;

    private float groupRadiusSqr;
    public float GroupRadiusSqr { get { return groupRadiusSqr; } }

    private List<Unit> units;

    void Awake()
    {
        units = new List<Unit>();
    }

    void Update()
    {
        // Update gather radius squared based on number of gathered units
        int numGathered = units.Where(unit => unit.State == UnitState.IDLE).Count();
        groupRadiusSqr = CalculateGroupRadiusSqr(numGathered);

        // If the user right clicks, move the selected units
        if (Input.GetButtonDown("Fire2"))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Get selected units
            units = GameManager.SelectionSystem.GetSelectionOfType<Unit>();

            // Set units destination
            foreach (Unit unit in units)
            {
                if (GameManager.SelectionSystem.HighlightedCluster != null)
                {
                    unit.Gather(GameManager.SelectionSystem.HighlightedCluster);
                }
                else
                {
                    unit.Relocate(mousePos);
                }
            }
        }
    }

    private float CalculateGroupRadiusSqr(int numGathered)
    {
        float gatheredArea = numGathered * UNIT_AREA;
        return Mathf.Max(minGroupRadiusSqr, gatheredArea / Mathf.PI);
    }
}
