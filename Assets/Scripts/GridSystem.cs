using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase groundTile;
    [SerializeField] private TileBase borderTile;
    [SerializeField] private int width = 40;
    [SerializeField] private int height = 30;
    [SerializeField] private int borderSize = 4;

    void Start()
    {
        if (width % 2 != 0 || height % 2 != 0)
        {
            Debug.LogWarning("Grid dimensions will behave unexpectedly with non-even values!");
        }

        tilemap.BoxFill(
            Vector3Int.zero,
            borderTile,
            -width / 2 - borderSize,
            -height / 2 - borderSize,
            width / 2 + borderSize,
            height / 2 + borderSize
        );
        tilemap.BoxFill(Vector3Int.zero, groundTile, -width / 2, -height / 2, width / 2, height / 2);
    }

    public Vector2Int GetDimensions()
    {
        return new Vector2Int(width, height);
    }
}
