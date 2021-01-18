using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionSystem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer boxSelectIndicatorPrefab;
    [SerializeField] private int maxMouseHoverTargets = 8;
    [SerializeField] private LayerMask selectionMask;

    private Transform indicatorParent;
    public Transform IndicatorParent { get { return indicatorParent; } }
    
    private bool isOverUI;
    public bool IsOverUI { get { return isOverUI; } }

    private ContactFilter2D selectionFilter;
    private List<Interactable> selection;
    private List<Interactable> hoverTargets;

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
        selection = new List<Interactable>();
        hoverTargets = new List<Interactable>();
        overlapResults = new List<Collider2D>();
        boxSelectIndicator = Instantiate(boxSelectIndicatorPrefab, indicatorParent);
        boxSelectIndicator.enabled = false;
    }

    void Update()
    {
        isOverUI = EventSystem.current.IsPointerOverGameObject();
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (GameManager.BuildingSystem.IsSelectionActive()) return;

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
            hoverTargets[i].Unhover();
        }
        hoverTargets.Clear();

        // Update hover targets based on selection type
        if (isBoxSelectActive && mousePos != boxSelectStartPos)
        {
            UpdateBoxHover();
        }
        else if (!isOverUI)
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
        hoverTargets = SelectionHelper.ConvertAll<Collider2D, Interactable>(overlapResults);
        hoverTargets = SelectionHelper.RemoveAll<Resource>(hoverTargets);

        // Update indicator box
        boxSelectIndicator.transform.position = boxCenter;
        boxSelectIndicator.size = boxSize;
    }

    private void UpdateMouseHover()
    {
        Collider2D mouseTarget = Physics2D.OverlapPoint(mousePos, selectionMask);
        if (mouseTarget == null) return;
 
        if (mouseTarget.TryGetComponent<Resource>(out Resource resource))
        {
            // Hover all resources in cluster
            List<Resource> resources = resource.Cluster.Resources;
            for (int i = 0; i < resources.Count; i++)
            {
                hoverTargets.Add(resources[i]);
            }
        }
        else if (mouseTarget.TryGetComponent<Interactable>(out Interactable interactable))
        {
            hoverTargets.Add(interactable);
        }
    }

    private void StartBoxSelect()
    {
        if (isOverUI) return;

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
        if (!Input.GetKey(KeyCode.LeftShift) && !isOverUI)
        {
            for (int i = 0; i < selection.Count; i++)
            {
                selection[i].Deselect();
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
            List<Unit> selectedUnits = SelectionHelper.ConvertAll<Interactable, Unit>(hoverTargets);
            selection.AddRange(selectedUnits);
            selection = selection.Distinct().ToList();
        }
        else if (SelectionHelper.ContainsAny<Building>(hoverTargets) || SelectionHelper.ContainsOnly<Building>(selection))
        {
            List<Building> selectedBuildings = SelectionHelper.ConvertAll<Interactable, Building>(hoverTargets);
            selection.AddRange(selectedBuildings);
            selection = selection.Distinct().ToList();
        }
        else if (SelectionHelper.ContainsAny<Resource>(hoverTargets) || SelectionHelper.ContainsOnly<Resource>(selection))
        {
            List<Resource> selectedResources = SelectionHelper.ConvertAll<Interactable, Resource>(hoverTargets);
            selection.AddRange(selectedResources);
            selection = selection.Distinct().ToList();
        }
    }

    public List<T> GetSelectionOfType<T>() where T : Interactable
    {
        return SelectionHelper.ConvertAll<Interactable, T>(selection);
    }

    public List<T> GetHoverTargetsOfType<T>() where T : Interactable
    {
        return SelectionHelper.ConvertAll<Interactable, T>(hoverTargets);
    }

    public void RemoveInteractable(Interactable interactable)
    {
        selection.Remove(interactable);
        hoverTargets.Remove(interactable);
    }

    private static class SelectionHelper
    {
        /// <summary>Returns whether a list contains any interactables of type T.</summary>
        public static bool ContainsAny<T>(List<Interactable> interactables) where T : Interactable
        {
            return interactables.Any(x => x.TryGetComponent<T>(out T t));
        }

        /// <summary>Returns whether a list contains only interactables of type T.</summary>
        public static bool ContainsOnly<T>(List<Interactable> interactables) where T : Interactable
        {
            return interactables.Any(x => x.TryGetComponent<T>(out T t));
        }

        /// <summary>Converts all <paramref name="components"/> of type C to interactables of type T.</summary>
        /// <returns>List of interactables of type T.</returns>
        public static List<T> ConvertAll<C, T>(List<C> components)
            where C : Component
            where T : Interactable
        {
            return components
                .Where(x => x.TryGetComponent<T>(out T t))
                .Select(x => x.GetComponent<T>())
                .ToList();
        }

        /// <summary>Filters out all interactables of type T from a list.</summary>
        public static List<Interactable> RemoveAll<T>(List<Interactable> interactables) where T : Interactable
        {
            return interactables.Where(x => !x.TryGetComponent<T>(out T t)).ToList();
        }
    }
}
