using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSystem : MonoBehaviour
{
    // Approximation of the area of a Unit with radius 0.5
    private const float UNIT_AREA = 0.8f;

    [SerializeField] private float minGatherRadiusSqr = 0.25f;

    private List<Unit> units;
    private float gatherRadiusSqr;

    void Awake()
    {
        units = new List<Unit>();
    }

    void Update()
    {
        // Update gather radius squared based on number of gathered units
        int numGathered = units.Where(unit => !unit.IsMoving()).Count();
        gatherRadiusSqr = CalculateGatherRadiusSqr(numGathered);

        // Update unit movement
        foreach (Unit unit in units)
        {
            unit.SetGatherRadiusSqr(gatherRadiusSqr);
        }

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
                    unit.AssignCluster(GameManager.SelectionSystem.HighlightedCluster);
                }
                else
                {
                    unit.UnassignCluster();
                    unit.SetGatherRadiusSqr(gatherRadiusSqr);
                    unit.SetMoveTarget(mousePos);
                }
            }
        }
    }

    private float CalculateGatherRadiusSqr(int numGathered)
    {
        float gatheredArea = numGathered * UNIT_AREA;
        return Mathf.Max(minGatherRadiusSqr, gatheredArea / Mathf.PI);
    }
}
