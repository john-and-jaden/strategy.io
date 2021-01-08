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
    private Cluster highlightedCluster;
    public Cluster HighlightedCluster { get { return highlightedCluster; } }
    private List<Interactable> hoverTargets;
    public List<Interactable> HoverTargets { get { return hoverTargets; } }

    private ContactFilter2D selectionFilter;
    private List<Interactable> selection;
    private List<Collider2D> overlapResults;
    private SpriteRenderer boxSelectIndicator;
    private Vector2 boxSelectStartPos;
    private bool isBoxSelectActive;

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
        // Cancel hover on previous selection
        for (int i = 0; i < hoverTargets.Count; i++)
        {
            hoverTargets[i].SetHovered(false);
        }

        // Calculate hover selection based on mouse position
        hoverTargets.Clear();
        highlightedCluster = null;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (isBoxSelectActive && mousePos != boxSelectStartPos)
        {
            // Get all objects within selection box
            Vector2 boxCenter = (boxSelectStartPos + mousePos) / 2;
            Vector2 boxDiff = mousePos - boxSelectStartPos;
            Vector2 boxSize = new Vector2(Mathf.Abs(boxDiff.x), Mathf.Abs(boxDiff.y));
            Physics2D.OverlapBox(boxCenter, boxSize, 0, selectionFilter, overlapResults);
            hoverTargets = FilterInteractables(overlapResults);

            // Update indicator box
            boxSelectIndicator.transform.position = boxCenter;
            boxSelectIndicator.size = boxSize;
        }
        else
        {
            // Get nearest object within range of cursor
            Physics2D.OverlapCircle(mousePos, selectDistance, selectionFilter, overlapResults);
            Interactable nearest = Helper.GetNearestInteractable(FilterInteractables(overlapResults), mousePos);
            if (nearest)
            {
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

    public List<T> GetSelectionOfType<T>() where T : Interactable
    {
        return selection.Select(s => s.GetComponent<T>()).ToList();
    }

    private List<Interactable> FilterInteractables(List<Collider2D> colliders)
    {
        return colliders.Where(c =>
        {
            Interactable s;
            return c.TryGetComponent<Interactable>(out s);
        }).Select(x =>
        {
            return x.GetComponent<Interactable>();
        }).ToList();
    }

    private bool ContainsType<T>(List<Interactable> interactables) where T : Interactable
    {
        return interactables.Any(s =>
        {
            T t;
            return s.TryGetComponent<T>(out t);
        });
    }

    private List<T> FilterType<T>(List<Interactable> interactables) where T : Interactable
    {
        return interactables.Where(x =>
        {
            T t;
            return x.TryGetComponent<T>(out t);
        }).Select(x =>
        {
            return x.GetComponent<T>();
        }).ToList();
    }

    private void HighlightCluster(Resource resource)
    {
        highlightedCluster = resource.Cluster;
        for (int i = 0; i < highlightedCluster.Resources.Count; i++)
        {
            hoverTargets.Add(highlightedCluster.Resources[i]);
        }
    }
}
