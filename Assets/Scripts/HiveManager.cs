using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum CellState
{
    Empty,
    Egg,
    Larva,
    Honey
}

public class HiveManager : MonoBehaviour
{
    public static HiveManager Instance;

    public Tilemap hiveTilemap;

    public TileBase emptyTile;
    public TileBase eggTile;
    public TileBase larvaTile;
    public TileBase honeyTile;

    public GameObject workerBeePrefab;

    // Tracks each cell's gameplay state
    public Dictionary<Vector3Int, CellState> cellStates = new Dictionary<Vector3Int, CellState>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Initialize all non-empty tiles as empty state
        foreach (var pos in hiveTilemap.cellBounds.allPositionsWithin)
        {
            if (hiveTilemap.HasTile(pos))
            {
                cellStates[pos] = CellState.Empty;
            }
        }
    }

    void Update()
    {
        HandleMouseClick();
    }

    void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector3Int cellPos = hiveTilemap.WorldToCell(mouseWorldPos);

        if (!cellStates.ContainsKey(cellPos)) return;

        // Try to place an egg on empty cells
        if (cellStates[cellPos] == CellState.Empty)
        {
            if (GameManager.Instance.SpendHoney(3))
            {
                PlaceEggAt(cellPos);
            }
            else
            {
                Debug.Log("Not enough honey to place egg.");
            }
        }
        // Try collecting honey from honey cells
        else if (cellStates[cellPos] == CellState.Honey)
        {
            if (CollectHoneyAt(cellPos))
            {
                Debug.Log("Collected honey.");
            }
        }
    }


    void PlaceEggAt(Vector3Int position)
    {
        cellStates[position] = CellState.Egg;
        UpdateTileVisual(position);

        // Spawn the bee at the tile’s world position
        Vector3 worldPos = hiveTilemap.CellToWorld(position) + new Vector3(0.5f, 0.5f, 0f);
        Instantiate(workerBeePrefab, worldPos, Quaternion.identity);
    }

    bool CollectHoneyAt(Vector3Int position)
    {
        if (cellStates[position] != CellState.Honey) return false;

        cellStates[position] = CellState.Empty;
        UpdateTileVisual(position);
        GameManager.Instance.AddHoney(5);
        return true;
    }

    public void UpdateTileVisual(Vector3Int position)
    {
        switch (cellStates[position])
        {
            case CellState.Empty:
                hiveTilemap.SetTile(position, emptyTile);
                break;
            case CellState.Honey:
                hiveTilemap.SetTile(position, honeyTile);
                break;
        }
    }

    // These allow bees to update tile visuals when they grow
    //public void SetTileToLarva(Vector3Int pos)
    //{
    //    if (!cellStates.ContainsKey(pos)) return;
    //    cellStates[pos] = CellState.Larva;
    //    UpdateTileVisual(pos);
    //}

    //public void SetTileToHoney(Vector3Int pos)
    //{
    //    if (!cellStates.ContainsKey(pos)) return;
    //    cellStates[pos] = CellState.Honey;
    //    UpdateTileVisual(pos);
    //}
}
