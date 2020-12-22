using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementIndicator : MonoBehaviour
{
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;

    private bool isPlacementValid;
    public bool IsPlacementValid { get { return isPlacementValid; } }

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        isPlacementValid = true;
        Hide();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("entered");
        isPlacementValid = false;
        spriteRenderer.color = invalidColor;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("exited");
        isPlacementValid = true;
        spriteRenderer.color = validColor;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void Show()
    {
        spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        spriteRenderer.enabled = false;
    }
}
