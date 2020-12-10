using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SelectionController))]
public class UnitController : MonoBehaviour
{
    // Approximation of the area of a Unit with radius 0.5
    private const float UNIT_AREA = 0.8f;

    public float minGatherRadiusSqr = 0.25f;

    private List<Unit> units;
    private float gatherRadiusSqr;

    private SelectionController selectionController;

    void Awake()
    {
        selectionController = GetComponent<SelectionController>();
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
            units = selectionController.GetSelectionOfType<Unit>();

            // ADD moving out of way based on collision
            foreach (Unit unit in units)
            {
                unit.SetMoveTarget(mousePos);
                unit.SetGatherRadiusSqr(gatherRadiusSqr);
                if (selectionController.highlightedCluster != null)
                {
                    unit.AssignedCluster = selectionController.highlightedCluster;
                    Debug.Log(unit.AssignedCluster.Id);
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
