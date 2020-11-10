using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private SelectionController selectionController;

    void Awake()
    {
        selectionController = GetComponent<SelectionController>();
    }

    void Update()
    {
        // If the user right clicks, move the selected units
        if (Input.GetButtonDown("Fire2"))
        {
            
        }
    }
}
