using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSystem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer boxSelectIndicatorPrefab;
    [SerializeField] private int maxMouseHoverTargets = 8;
    [SerializeField] private LayerMask selectionMask;

    private Transform indicatorParent;
    public Transform IndicatorParent { get { return indicatorParent; } }

    // TODO: refactor this
    private Cluster highlightedCluster;
    public Cluster HighlightedCluster { get { return highlightedCluster; } }

    private ContactFilter2D selectionFilter;
    private List<Selectable> selection;
    private List<Selectable> hoverTargets;
    private List<Collider2D> overlapResults;
    private SpriteRenderer boxSelectIndicator;
    private Vector2 boxSelectStartPos;
    private bool isBoxSelectActive;
    private Vector2 mousePos;

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
        // Update the mouse position
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Update the current hover targets
        UpdateHoverTargets();

        // Start selection box on left-click pressed
        if (Input.GetButtonDown("Fire1"))
        {
            StartBoxSelect();
        }

        // Select hovered objects on left-click released
        if (Input.GetButtonUp("Fire1"))
        {
            SelectHovered();
        }
    }

    private void UpdateHoverTargets()
    {
        // Cancel hover on previous targets
        for (int i = 0; i < hoverTargets.Count; i++)
        {
            hoverTargets[i].CancelHover();
        }
        hoverTargets.Clear();

        // TODO: refactor this
        highlightedCluster = null;

        // Update hover targets based on selection type
        if (isBoxSelectActive && mousePos != boxSelectStartPos)
        {
            UpdateBoxHover();
        }
        else
        {
            UpdateMouseHover();
        }

        // Update hover state for new targets
        for (int i = 0; i < hoverTargets.Count; i++)
        {
            hoverTargets[i].Hover();
        }
    }

    private void UpdateBoxHover()
    {
        // Get all objects within selection box
        Vector2 boxCenter = (boxSelectStartPos + mousePos) / 2;
        Vector2 boxDiff = mousePos - boxSelectStartPos;
        Vector2 boxSize = new Vector2(Mathf.Abs(boxDiff.x), Mathf.Abs(boxDiff.y));
        Physics2D.OverlapBox(boxCenter, boxSize, 0, selectionFilter, overlapResults);
        hoverTargets = SelectionHelper.Convert<Collider2D, Selectable>(overlapResults);

        // Update indicator box
        boxSelectIndicator.transform.position = boxCenter;
        boxSelectIndicator.size = boxSize;
    }

    private void UpdateMouseHover()
    {
        // Get nearest object within range of cursor
        Physics2D.OverlapPoint(mousePos, selectionFilter, overlapResults);
        List<Selectable> targetsInRange = SelectionHelper.Convert<Collider2D, Selectable>(overlapResults);
        Selectable nearest = SelectionHelper.GetNearest(targetsInRange, mousePos);
        if (nearest != null)
        {
            // TODO: refactor this
            if (nearest.GetType().IsSubclassOf(typeof(Resource)))
            {
                HighlightCluster((Resource)nearest);
            }
            else
            {
                hoverTargets.Add(nearest);
            }
        }
    }

    private void StartBoxSelect()
    {
        isBoxSelectActive = true;
        boxSelectIndicator.enabled = true;
        boxSelectIndicator.transform.position = mousePos;
        boxSelectIndicator.size = Vector2.zero;
        boxSelectStartPos = mousePos;
    }

    private void SelectHovered()
    {
        // Cancel the selection box
        isBoxSelectActive = false;
        boxSelectIndicator.enabled = false;

        // Cancel previous selections if not holding shift
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            for (int i = 0; i < selection.Count; i++)
            {
                selection[i].CancelSelect();
            }
            selection.Clear();
        }

        // Add hover targets to selection list by priority
        SelectByPriority();

        // Update selection states
        for (int i = 0; i < selection.Count; i++)
        {
            selection[i].Select();
        }
    }

    private void SelectByPriority()
    {
        // We could make this generic using type reflection, however that would make the code substantially more confusing
        if (SelectionHelper.ContainsAny<Unit>(hoverTargets) || SelectionHelper.ContainsOnly<Unit>(selection))
        {
            List<Unit> selectedUnits = SelectionHelper.Convert<Selectable, Unit>(hoverTargets);
            selection.AddRange(selectedUnits);
            selection = selection.Distinct().ToList();
        }
        else if (SelectionHelper.ContainsAny<Building>(hoverTargets) || SelectionHelper.ContainsOnly<Building>(selection))
        {
            List<Building> selectedBuildings = SelectionHelper.Convert<Selectable, Building>(hoverTargets);
            selection.AddRange(selectedBuildings);
            selection = selection.Distinct().ToList();
        }
        else if (SelectionHelper.ContainsAny<Resource>(hoverTargets) || SelectionHelper.ContainsOnly<Resource>(selection))
        {
            List<Resource> selectedResources = SelectionHelper.Convert<Selectable, Resource>(hoverTargets);
            selection.AddRange(selectedResources);
            selection = selection.Distinct().ToList();
        }
    }

    public List<T> GetSelectionOfType<T>() where T : Selectable
    {
        return SelectionHelper.Convert<Selectable, T>(selection);
    }

    // TODO: refactor this
    private void HighlightCluster(Resource resource)
    {
        highlightedCluster = resource.Cluster;
        for (int i = 0; i < highlightedCluster.Resources.Count; i++)
        {
            hoverTargets.Add(highlightedCluster.Resources[i]);
        }
    }

    private static class SelectionHelper
    {
        /// <summary>Returns whether a list contains any selectables of type S.</summary>
        public static bool ContainsAny<S>(List<Selectable> selectables) where S : Selectable
        {
            return selectables.Any(s => s.TryGetComponent<S>(out S t));
        }

        /// <summary>Returns whether a list contains only selectables of type S.</summary>
        public static bool ContainsOnly<S>(List<Selectable> selectables) where S : Selectable
        {
            return selectables.Any(s => s.TryGetComponent<S>(out S t));
        }

        /// <summary>Gets all Selectable components of type S from <paramref name="components"/>.</summary>
        /// <returns>List of selectables of type S.</returns>
        public static List<S> Convert<C, S>(List<C> components)
            where C : Component
            where S : Selectable
        {
            return components
                .Where(x => x.TryGetComponent<S>(out S s))
                .Select(x => x.GetComponent<S>())
                .ToList();
        }

        /// <summary>Returns the nearest Selectable from <paramref name="components"/>
        /// to the given <paramref name="targetPos"/>.</summary>
        public static Selectable GetNearest(List<Selectable> components, Vector2 targetPos)
        {
            int nearestIdx = -1;
            float minDist = float.MaxValue;
            for (int i = 0; i < components.Count; i++)
            {
                Vector2 dir = targetPos - (Vector2)components[i].transform.position;
                float sqrDist = dir.sqrMagnitude;
                if (sqrDist < minDist)
                {
                    nearestIdx = i;
                    minDist = sqrDist;
                }
            }
            return nearestIdx >= 0 ? components[nearestIdx] : null;
        }
    }
}
