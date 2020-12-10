using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSystem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer boxSelectIndicatorPrefab;
    [SerializeField] private float selectDistance = 0.2f;
    [SerializeField] private int maxMouseHoverTargets = 8;
    [SerializeField] private LayerMask selectionMask;

    private Transform indicatorParent;
    public Transform IndicatorParent { get { return indicatorParent; } }

    private ContactFilter2D selectionFilter;
    private List<Selectable> selection;
    private List<Selectable> hoverTargets;
    private List<Collider2D> overlapResults;
    private SpriteRenderer boxSelectIndicator;
    private Vector2 boxSelectStartPos;
    private bool isBoxSelectActive;

    void Awake()
    {
        indicatorParent = new GameObject("Selection Indicators").transform;
        selectionFilter = new ContactFilter2D();
        selectionFilter.layerMask = selectionMask;
        selection = new List<Selectable>();
        hoverTargets = new List<Selectable>();
        overlapResults = new List<Collider2D>();
        boxSelectIndicator = Instantiate(boxSelectIndicatorPrefab, indicatorParent);
        boxSelectIndicator.enabled = false;
    }

    void Update()
    {
        // Cancel hover on previous selection
        for (int i = 0; i < hoverTargets.Count; i++)
        {
            hoverTargets[i].SetHovered(false);
        }

        // Calculate hover selection based on mouse position
        hoverTargets.Clear();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (isBoxSelectActive && mousePos != boxSelectStartPos)
        {
            // Get all objects within selection box
            Vector2 boxCenter = (boxSelectStartPos + mousePos) / 2;
            Vector2 boxDiff = mousePos - boxSelectStartPos;
            Vector2 boxSize = new Vector2(Mathf.Abs(boxDiff.x), Mathf.Abs(boxDiff.y));
            Physics2D.OverlapBox(boxCenter, boxSize, 0, selectionFilter, overlapResults);
            hoverTargets = FilterSelectables(overlapResults);

            // Update indicator box
            boxSelectIndicator.transform.position = boxCenter;
            boxSelectIndicator.size = boxSize;
        }
        else
        {
            // Get nearest object within range of cursor
            Physics2D.OverlapCircle(mousePos, selectDistance, selectionFilter, overlapResults);
            Selectable nearest = GetNearestSelectable(FilterSelectables(overlapResults), mousePos);
            if (nearest)
            {
                hoverTargets.Add(nearest);
            }
        }

        // Update hover state
        for (int i = 0; i < hoverTargets.Count; i++)
        {
            hoverTargets[i].SetHovered(true);
        }

        // Start selection box on left-click pressed
        if (Input.GetButtonDown("Fire1"))
        {
            isBoxSelectActive = true;
            boxSelectIndicator.enabled = true;
            boxSelectIndicator.transform.position = mousePos;
            boxSelectIndicator.size = Vector2.zero;
            boxSelectStartPos = mousePos;
        }

        // Select hovered objects on left-click released
        if (Input.GetButtonUp("Fire1"))
        {
            // Cancel the selection box
            isBoxSelectActive = false;
            boxSelectIndicator.enabled = false;

            // Cancel previous selections if not holding shift
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                for (int i = 0; i < selection.Count; i++)
                {
                    selection[i].SetSelected(false);
                }
                selection.Clear();
            }

            // Add hover targets to selection list
            if (ContainsType<Unit>(hoverTargets) || ContainsType<Unit>(selection))
            {
                selection.AddRange(FilterType<Unit>(hoverTargets));
                selection = selection.Distinct().ToList();
            }
            else if (ContainsType<Building>(hoverTargets) && !ContainsType<Unit>(selection))
            {
                selection.AddRange(FilterType<Building>(hoverTargets));
                selection = selection.Distinct().ToList();
            }

            // Update selection states
            for (int i = 0; i < selection.Count; i++)
            {
                selection[i].SetSelected(true);
            }
        }
    }

    public List<T> GetSelectionOfType<T>() where T : Selectable
    {
        return selection.Select(s => s.GetComponent<T>()).ToList();
    }

    private List<Selectable> FilterSelectables(List<Collider2D> colliders)
    {
        return colliders.Where(c =>
        {
            Selectable s;
            return c.TryGetComponent<Selectable>(out s);
        }).Select(x =>
        {
            return x.GetComponent<Selectable>();
        }).ToList();
    }

    private bool ContainsType<T>(List<Selectable> selectables) where T : Selectable
    {
        return selectables.Any(s =>
        {
            T t;
            return s.TryGetComponent<T>(out t);
        });
    }

    private List<T> FilterType<T>(List<Selectable> selectables) where T : Selectable
    {
        return selectables.Where(x =>
        {
            T t;
            return x.TryGetComponent<T>(out t);
        }).Select(x =>
        {
            return x.GetComponent<T>();
        }).ToList();
    }

    private Selectable GetNearestSelectable(List<Selectable> selectables, Vector2 targetPos)
    {
        int nearestIdx = -1;
        float smallestDist = selectDistance * selectDistance + 1;
        for (int i = 0; i < selectables.Count; i++)
        {
            Vector2 dir = targetPos - (Vector2)selectables[i].transform.position;
            float sqrDist = dir.sqrMagnitude;
            if (sqrDist < smallestDist)
            {
                nearestIdx = i;
                smallestDist = sqrDist;
            }
        }
        return nearestIdx >= 0 ? selectables[nearestIdx] : null;
    }
}
