using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public float selectDistance = 1f;
    public int maxHoverTargets = 8;
    public LayerMask hoverMask;
    public SpriteRenderer hoverIndicatorPrefab;

    private ContactFilter2D hoverFilter;
    private Collider2D[] hoverTargets;
    private SpriteRenderer[] hoverIndicators;

    void Start()
    {
        hoverFilter = new ContactFilter2D();
        hoverFilter.layerMask = hoverMask;
        hoverTargets = new Collider2D[maxHoverTargets];
        hoverIndicators = new SpriteRenderer[maxHoverTargets];
        for (int i = 0; i < maxHoverTargets; i++)
        {
            hoverIndicators[i] = Instantiate(hoverIndicatorPrefab);
        }
    }

    void Update()
    {
        // Get all units within range
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int hoverTargetCount = Physics2D.OverlapCircle(mousePos, selectDistance, hoverFilter, hoverTargets);

        // Get the nearest targeted unit
        int nearestIndex = 0;
        float smallestDist = selectDistance + 1;
        for (int i = 0; i < hoverTargetCount; i++)
        {
            float dist = Vector2.Distance(hoverTargets[i].transform.position, mousePos);
            if (dist < smallestDist)
            {
                nearestIndex = i;
                smallestDist = dist;
            }
        }
        
        // Set the hover states
        for (int i = 0; i < maxHoverTargets; i++)
        {
            SpriteRenderer indicator = hoverIndicators[i];
            if (i == nearestIndex)
            {
                indicator.transform.position = hoverTargets[i].transform.position;
            }
            indicator.enabled = i == nearestIndex;
        }
        
        // Select this object on left-click
        if (Input.GetButtonDown("Fire1"))
        {
            
        }
    }
}
